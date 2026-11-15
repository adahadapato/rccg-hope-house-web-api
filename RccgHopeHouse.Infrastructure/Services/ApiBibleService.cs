using System.Net.Http.Json;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Infrastructure implementation of <see cref="IApiBibleService"/> for
/// communicating with the external API.Bible REST service.
/// </summary>
/// <remarks>
/// The HttpClient is configured by Infrastructure dependency injection with
/// API.Bible's base address and API key.
///
/// API.Bible-specific response DTOs remain internal to this integration and
/// are mapped to Core models before data leaves the Infrastructure layer.
/// </remarks>
public sealed class ApiBibleService : IApiBibleService
{
    private readonly HttpClient _httpClient;

    public ApiBibleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves English Bible translations available to the configured
    /// API.Bible account.
    /// </summary>
    public async Task<IReadOnlyList<ApiBibleTranslation>> GetAvailableBiblesAsync(
        CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.GetFromJsonAsync<ApiBibleListResponseDto>(
                "bibles?language=eng",
                cancellationToken);

        if (response is null)
        {
            return [];
        }

        return response.Data
            .Select(bible => new ApiBibleTranslation(
                Code: bible.Id,
                Name: bible.Name,
                Abbreviation: bible.Abbreviation))
            .ToList();
    }

    /// <summary>
    /// Retrieves a specific passage from an API.Bible translation.
    /// </summary>
    /// <remarks>
    /// The passage identifier must use API.Bible's USFM-style format,
    /// for example EXO.14.1-EXO.14.4 for Exodus 14:1-4.
    ///
    /// Text content is requested so that provider-specific HTML is not
    /// propagated into the Core or Application layers.
    /// </remarks>
    public async Task<ApiBiblePassage> GetPassageAsync(
        string bibleId,
        string passageId,
        CancellationToken cancellationToken = default)
    {
        var url =
            $"bibles/{Uri.EscapeDataString(bibleId)}/passages/" +
            $"{Uri.EscapeDataString(passageId)}?content-type=text";

        var response =
            await _httpClient.GetFromJsonAsync<ApiBiblePassageResponseDto>(
                url,
                cancellationToken);

        if (response?.Data is null)
        {
            throw new InvalidOperationException(
                "API.Bible returned an empty passage response.");
        }

        return new ApiBiblePassage(
                BibleId: response.Data.BibleId,
                Reference: response.Data.Reference,
                Content: response.Data.Content,
                Copyright: response.Data.Copyright);
    }
}


// ==================== API.Bible Transport DTOs ====================

/// <summary>
/// Represents API.Bible's response to GET /bibles.
/// </summary>
internal sealed class ApiBibleListResponseDto
{
    public List<ApiBibleItemDto> Data { get; set; } = [];
}

/// <summary>
/// Represents the subset of an API.Bible Bible resource required by
/// Hope House.
/// </summary>
internal sealed class ApiBibleItemDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Abbreviation { get; set; } = string.Empty;
}

/// <summary>
/// Represents the top-level response returned by API.Bible when requesting
/// a passage.
/// </summary>
internal sealed class ApiBiblePassageResponseDto
{
    public ApiBiblePassageDataDto? Data { get; set; }
}

/// <summary>
/// Represents the API.Bible passage data required by Hope House.
/// Additional provider fields are intentionally ignored.
/// </summary>
internal sealed class ApiBiblePassageDataDto
{
    public string BibleId { get; set; } = string.Empty;

    public string Reference { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Copyright { get; set; } = string.Empty;
}