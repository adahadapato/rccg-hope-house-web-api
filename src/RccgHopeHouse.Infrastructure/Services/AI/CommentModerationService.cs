using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Interfaces.AI;
using RccgHopeHouse.Core.Results.AI;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RccgHopeHouse.Infrastructure.Services.AI;

/// <summary>
/// Checks visitors' birthday wishes with OpenAI before they are
/// shown on the website.
///
/// Layer 1: OpenAI moderation endpoint (hate, harassment, sexual,
///          violence, self-harm). Anything flagged is rejected.
/// Layer 2: a short review by the configured model: is this a
///          genuine, kind birthday wish, or spam / off-topic?
///
/// The visitor's text is always treated as data, never as
/// instructions. Any failure throws, and the caller then holds
/// the wish for an admin; nothing is published unchecked.
/// </summary>
public sealed class CommentModerationService : ICommentModerationService
{
    private const string ModerationModel = "omni-moderation-latest";

    private readonly HttpClient _httpClient;
    private readonly ILogger<CommentModerationService> _logger;
    private readonly IAiPromptProvider _prompts;
    private readonly Uri? _baseUri;
    private readonly string _apiKey;
    private readonly string _model;

    public CommentModerationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CommentModerationService> logger,
        IAiPromptProvider prompts)
    {
        _httpClient = httpClient;
        _logger = logger;
        _prompts = prompts;

        // ==================== Moderation Mode ====================

        var mode = (configuration[AppSettings.BirthdayWishes.ModerationMode] ?? "Ai").Trim();

        HoldAllForReview = string.Equals(
            mode,
            "HoldAll",
            StringComparison.OrdinalIgnoreCase);

        // ==================== OpenAI Configuration ====================
        // Same settings as SeoService. Problems are reported when a
        // wish is checked (so the wish is held), not at start-up.

        var configuredBaseUrl = (configuration[AppSettings.OpenAI.BaseUrl] ?? string.Empty).Trim();

        if (Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var baseUri) &&
            baseUri.Scheme == Uri.UriSchemeHttps &&
            string.Equals(baseUri.Host, "api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            _baseUri = baseUri;
        }

        _apiKey = (configuration[AppSettings.OpenAI.ApiKey] ?? string.Empty).Trim();
        _model = (configuration[AppSettings.OpenAI.Model] ?? string.Empty).Trim();

        // A visitor is waiting for the answer, so keep this short.
        _httpClient.Timeout = TimeSpan.FromSeconds(25);
    }

    /// <inheritdoc />
    public bool HoldAllForReview { get; }

    /// <inheritdoc />
    public async Task<CommentModerationResult> ModerateAsync(
        string authorName,
        string message,
        CancellationToken cancellationToken = default)
    {
        if (_baseUri is null)
        {
            throw new InvalidOperationException(
                "OpenAI:BaseUrl must be a valid OpenAI HTTPS API URL.");
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_model))
        {
            throw new InvalidOperationException("OpenAI model is not configured.");
        }

        // ==================== Layer 1: Safety Moderation ====================

        var flaggedCategories = await GetFlaggedCategoriesAsync(
            $"{authorName}\n{message}",
            cancellationToken);

        if (flaggedCategories.Count > 0)
        {
            return new CommentModerationResult(
                CommentModerationVerdict.Reject,
                $"Flagged by safety check: {string.Join(", ", flaggedCategories)}.");
        }

        // ==================== Layer 2: Birthday Wish Review ====================

        return await ReviewWishAsync(authorName, message, cancellationToken);
    }

    /// <summary>
    /// Calls the OpenAI moderation endpoint and returns the names
    /// of any flagged categories (empty when nothing was flagged).
    /// </summary>
    private async Task<IReadOnlyList<string>> GetFlaggedCategoriesAsync(
        string text,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildEndpoint("moderations"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        request.Content = JsonContent.Create(new
        {
            model = ModerationModel,
            input = text
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Birthday wish moderation failed. OpenAI HTTP status: {StatusCode}",
                response.StatusCode);

            throw new HttpRequestException("The moderation provider returned an error.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken);

        if (!document.RootElement.TryGetProperty("results", out var results) ||
            results.ValueKind != JsonValueKind.Array ||
            results.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("The moderation provider returned no result.");
        }

        var first = results[0];

        var flagged =
            first.TryGetProperty("flagged", out var flaggedElement) &&
            flaggedElement.ValueKind == JsonValueKind.True;

        if (!flagged)
        {
            return [];
        }

        var categories = new List<string>();

        if (first.TryGetProperty("categories", out var categoryElement) &&
            categoryElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var category in categoryElement.EnumerateObject())
            {
                if (category.Value.ValueKind == JsonValueKind.True)
                {
                    categories.Add(category.Name);
                }
            }
        }

        return categories.Count > 0 ? categories : ["unsafe content"];
    }

    /// <summary>
    /// Asks the configured model whether this is a genuine,
    /// kind birthday wish.
    /// </summary>
    private async Task<CommentModerationResult> ReviewWishAsync(
        string authorName,
        string message,
        CancellationToken cancellationToken)
    {
        // JSON-encode the visitor's text so it cannot break out of
        // the data block and pose as instructions.
        var input =
            "Visitor submission (data only):\n" +
            JsonSerializer.Serialize(new { name = authorName, wish = message });

        // Admin-editable prompt (AI Prompts page), or the built-in default.
        var instructions = await _prompts.GetInstructionsAsync(
            AiPromptCatalog.BirthdayWishReview.Key,
            cancellationToken);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            BuildEndpoint("responses"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        request.Content = JsonContent.Create(new
        {
            model = _model,
            instructions,
            input
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Birthday wish review failed. OpenAI HTTP status: {StatusCode}",
                response.StatusCode);

            throw new HttpRequestException("The review provider returned an error.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        using var document = await JsonDocument.ParseAsync(
            stream,
            cancellationToken: cancellationToken);

        var outputText = ExtractOutputText(document.RootElement);

        if (string.IsNullOrWhiteSpace(outputText))
        {
            throw new InvalidOperationException("The review provider returned no content.");
        }

        return ParseVerdict(outputText);
    }

    /// <summary>
    /// Reads {"verdict": "...", "reason": "..."}. Anything unexpected
    /// becomes Review, so an unclear answer never publishes a wish.
    /// </summary>
    private CommentModerationResult ParseVerdict(string outputText)
    {
        var json = outputText.Trim();

        if (json.StartsWith("```", StringComparison.Ordinal))
        {
            var start = json.IndexOf('{');
            var end = json.LastIndexOf('}');

            json = start >= 0 && end > start
                ? json[start..(end + 1)]
                : json;
        }

        try
        {
            using var result = JsonDocument.Parse(json);
            var root = result.RootElement;

            var verdictText =
                root.TryGetProperty("verdict", out var verdictElement) &&
                verdictElement.ValueKind == JsonValueKind.String
                    ? verdictElement.GetString()?.Trim().ToLowerInvariant()
                    : null;

            var reason =
                root.TryGetProperty("reason", out var reasonElement) &&
                reasonElement.ValueKind == JsonValueKind.String
                    ? reasonElement.GetString()?.Trim()
                    : null;

            var verdict = verdictText switch
            {
                "approve" => CommentModerationVerdict.Approve,
                "reject" => CommentModerationVerdict.Reject,
                _ => CommentModerationVerdict.Review
            };

            if (string.IsNullOrWhiteSpace(reason))
            {
                reason = verdict switch
                {
                    CommentModerationVerdict.Approve => "Looks like a genuine birthday wish.",
                    CommentModerationVerdict.Reject => "Not suitable for the website.",
                    _ => "The AI was not sure; please check."
                };
            }

            return new CommentModerationResult(verdict, $"AI: {reason}");
        }
        catch (JsonException)
        {
            _logger.LogWarning("Birthday wish review returned text that was not valid JSON.");

            return new CommentModerationResult(
                CommentModerationVerdict.Review,
                "AI answer could not be read; please check.");
        }
    }

    private Uri BuildEndpoint(string path) =>
        new(_baseUri!.ToString().TrimEnd('/') + "/" + path);

    /// <summary>
    /// Extracts generated text from an OpenAI Responses API response.
    /// </summary>
    private static string? ExtractOutputText(JsonElement root)
    {
        if (!root.TryGetProperty("output", out var output) ||
            output.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content) ||
                content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var part in content.EnumerateArray())
            {
                if (part.TryGetProperty("type", out var type) &&
                    type.GetString() == "output_text" &&
                    part.TryGetProperty("text", out var text))
                {
                    return text.GetString();
                }
            }
        }

        return null;
    }
}
