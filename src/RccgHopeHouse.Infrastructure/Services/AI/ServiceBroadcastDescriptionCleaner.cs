using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private const string Instructions =
    """
    You are an editor for a church website.

    You will receive the title and description of a YouTube church
    service broadcast.

    Perform two tasks:

    1. Clean the description.
    2. Extract the service theme.

    DESCRIPTION RULES:

    Keep only meaningful information directly describing the church
    service, such as:
    - the purpose or nature of the service
    - worship
    - prayer
    - the Word or sermon
    - Holy Communion
    - the service theme
    - the minister or speaker
    - other meaningful information directly related to the service

    ALWAYS REMOVE:
    - like, share and subscribe requests
    - calls to follow or stay connected
    - YouTube channel promotion
    - television channel information
    - GOTV, DSTV or other TV platform information
    - mobile app promotion
    - Android or iOS app information
    - websites and URLs
    - social media accounts and handles
    - donation or giving information
    - promotional material
    - copyright statements
    - disclaimers
    - hashtags
    - unrelated announcements
    - repeated information
    - symbols or formatting used for promotional links

    Do not summarize promotional material.
    Remove it completely.

    Write the cleaned description as one concise paragraph suitable
    for display on a church website.

    THEME RULES:

    Extract the theme only when it is explicitly stated or clearly
    identified in the supplied title or description.

    Examples include:
    "Theme: ACCEPTABLE SERVICE"
    "with the theme ACCEPTABLE SERVICE"
    "THEME - ACCEPTABLE SERVICE"

    Return only the theme itself.

    Do not invent a theme.
    Do not infer a theme merely from the service title.
    If no theme is explicitly identifiable, return null.

    GENERAL RULES:

    Do not invent facts.
    Do not add names, speakers, themes or details that are not present
    in the supplied information.

    Return ONLY valid JSON in exactly this structure:

    {
      "description": "cleaned description",
      "theme": "theme or null"
    }

    The theme property must be null when no theme is identified.
    Do not include markdown, code fences, headings or commentary.
    """;

    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceBroadcastDescriptionCleaner> _logger;
    private readonly string _apiKey;
    private readonly string _model;

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
    public ServiceBroadcastDescriptionCleaner(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ServiceBroadcastDescriptionCleaner> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

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
            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.openai.com/v1/responses");

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
                        instructions = Instructions,
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