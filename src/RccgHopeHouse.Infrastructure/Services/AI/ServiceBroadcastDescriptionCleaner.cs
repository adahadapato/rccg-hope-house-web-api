using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.AI;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Interfaces.AI;

namespace RccgHopeHouse.Infrastructure.Services.AI;

/// <summary>
/// Cleans service broadcast descriptions and extracts service themes
/// using the OpenAI API.
/// </summary>
public sealed class ServiceBroadcastDescriptionCleaner
    : IServiceBroadcastDescriptionCleaner
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceBroadcastDescriptionCleaner> _logger;
    private readonly IAiPromptProvider _prompts;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly Uri _openAiEndpoint;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ServiceBroadcastDescriptionCleaner"/> class.
    /// </summary>
    /// <param name="httpClient">
    /// HTTP client used to communicate with OpenAI.
    /// </param>
    /// <param name="configuration">
    /// Application configuration.
    /// </param>
    /// <param name="logger">
    /// Logger used for diagnostic information.
    /// </param>
    /// <param name="prompts">
    /// Supplies the admin-editable prompt (AI Prompts page).
    /// </param>
    public ServiceBroadcastDescriptionCleaner(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ServiceBroadcastDescriptionCleaner> logger,
        IAiPromptProvider prompts)
    {
        _httpClient = httpClient;
        _logger = logger;
        _prompts = prompts;

        _apiKey =
            (configuration[AppSettings.OpenAI.ApiKey] ??
             string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\0", string.Empty)
            .Trim();

        _model =
            configuration[AppSettings.OpenAI.Model]?.Trim()
            ?? string.Empty;

        var openAiBaseUrl = configuration[AppSettings.OpenAI.BaseUrl]?.Trim();
        if (!Uri.TryCreate(openAiBaseUrl, UriKind.Absolute, out var openAiBaseUri) ||
            openAiBaseUri.Scheme != Uri.UriSchemeHttps ||
            !string.Equals(openAiBaseUri.Host, "api.openai.com", StringComparison.OrdinalIgnoreCase) ||
            openAiBaseUri.AbsolutePath.TrimEnd('/') != "/v1" ||
            !string.IsNullOrEmpty(openAiBaseUri.Query) ||
            !string.IsNullOrEmpty(openAiBaseUri.Fragment))
        {
            throw new InvalidOperationException(
                "OpenAI:BaseUrl must be https://api.openai.com/v1/.");
        }

        _openAiEndpoint = new Uri(openAiBaseUri.ToString().TrimEnd('/') + "/responses");
    }

    /// <inheritdoc />
    public async Task<ServiceBroadcastCleaningResult> CleanAsync(
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var originalDescription =
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();

        var normalizedTitle =
            string.IsNullOrWhiteSpace(title)
                ? string.Empty
                : title.Trim();

        if (string.IsNullOrWhiteSpace(normalizedTitle) &&
            originalDescription is null)
        {
            return new ServiceBroadcastCleaningResult(
                null,
                null);
        }

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogWarning(
                "OpenAI API key is not configured. " +
                "The original broadcast description will be used.");

            return new ServiceBroadcastCleaningResult(
                originalDescription,
                null);
        }

        if (string.IsNullOrWhiteSpace(_model))
        {
            _logger.LogWarning(
                "OpenAI model is not configured. " +
                "The original broadcast description will be used.");

            return new ServiceBroadcastCleaningResult(
                originalDescription,
                null);
        }

        try
        {
            // Admin-editable prompt (AI Prompts page), or the built-in default.
            var instructions = await _prompts.GetInstructionsAsync(
                AiPromptCatalog.ServiceBroadcastCleaner.Key,
                cancellationToken);

            using var request =
                new HttpRequestMessage(HttpMethod.Post, _openAiEndpoint);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _apiKey);

            var input =
                $"""
                TITLE:
                {normalizedTitle}

                DESCRIPTION:
                {originalDescription ?? string.Empty}
                """;

            request.Content =
                JsonContent.Create(
                    new
                    {
                        model = _model,
                        instructions,
                        input
                    });

            using var response =
                await _httpClient.SendAsync(
                    request,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                _logger.LogWarning(
                    "OpenAI broadcast cleaning failed with " +
                    "status {StatusCode}. Response: {Response}",
                    response.StatusCode,
                    error);

                return new ServiceBroadcastCleaningResult(
                    originalDescription,
                    null);
            }

            await using var stream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            using var document =
                await JsonDocument.ParseAsync(
                    stream,
                    cancellationToken:
                        cancellationToken);

            var outputText =
                ExtractOutputText(
                    document.RootElement);

            if (string.IsNullOrWhiteSpace(outputText))
            {
                _logger.LogWarning(
                    "OpenAI returned no broadcast metadata. " +
                    "The original broadcast description will be used.");

                return new ServiceBroadcastCleaningResult(
                    originalDescription,
                    null);
            }

            return ParseCleaningResult(
                outputText,
                originalDescription);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "OpenAI broadcast cleaning failed. " +
                "The original broadcast description will be used.");

            return new ServiceBroadcastCleaningResult(
                originalDescription,
                null);
        }
    }

    /// <summary>
    /// Parses the JSON generated by OpenAI.
    /// </summary>
    /// <param name="outputText">
    /// JSON text returned by OpenAI.
    /// </param>
    /// <param name="originalDescription">
    /// Original description used when the returned description
    /// cannot be read.
    /// </param>
    /// <returns>
    /// Parsed cleaned description and theme.
    /// </returns>
    private ServiceBroadcastCleaningResult ParseCleaningResult(
        string outputText,
        string? originalDescription)
    {
        try
        {
            using var document =
                JsonDocument.Parse(
                    outputText.Trim());

            var root =
                document.RootElement;

            string? cleanedDescription = null;
            string? theme = null;

            if (root.TryGetProperty(
                    "description",
                    out var descriptionElement) &&
                descriptionElement.ValueKind ==
                JsonValueKind.String)
            {
                cleanedDescription =
                    NormalizeOptionalText(
                        descriptionElement.GetString());
            }

            if (root.TryGetProperty(
                    "theme",
                    out var themeElement) &&
                themeElement.ValueKind ==
                JsonValueKind.String)
            {
                theme =
                    NormalizeOptionalText(
                        themeElement.GetString());
            }

            return new ServiceBroadcastCleaningResult(
                cleanedDescription ??
                originalDescription,
                theme);
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(
                exception,
                "OpenAI returned invalid broadcast metadata JSON. " +
                "The original broadcast description will be used.");

            return new ServiceBroadcastCleaningResult(
                originalDescription,
                null);
        }
    }

    /// <summary>
    /// Extracts generated output text from an OpenAI Responses API
    /// response.
    /// </summary>
    /// <param name="root">
    /// Root JSON element returned by OpenAI.
    /// </param>
    /// <returns>
    /// Generated text, or <see langword="null"/> when no text
    /// could be found.
    /// </returns>
    private static string? ExtractOutputText(
        JsonElement root)
    {
        if (!root.TryGetProperty(
                "output",
                out var output) ||
            output.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty(
                    "content",
                    out var content) ||
                content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (!contentItem.TryGetProperty(
                        "type",
                        out var type) ||
                    !string.Equals(
                        type.GetString(),
                        "output_text",
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (contentItem.TryGetProperty(
                        "text",
                        out var text))
                {
                    return text.GetString();
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Normalizes optional text.
    /// </summary>
    /// <param name="value">
    /// Text to normalize.
    /// </param>
    /// <returns>
    /// Trimmed text, or <see langword="null"/> when empty.
    /// </returns>
    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}