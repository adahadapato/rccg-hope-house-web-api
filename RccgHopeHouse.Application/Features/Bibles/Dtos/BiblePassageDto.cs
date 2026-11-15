namespace RccgHopeHouse.Application.Features.Bibles.Dtos;

/// <summary>
/// Represents a Bible passage returned by the Hope House API.
/// </summary>
/// <param name="BibleId">
/// Identifier of the Bible translation/resource used for the passage.
/// </param>
/// <param name="Reference">
/// Human-readable scripture reference, for example Exodus 14:1-4.
/// </param>
/// <param name="Content">
/// Scripture text.
/// </param>
/// <param name="Copyright">
/// Copyright or attribution information supplied by the Bible provider.
/// </param>
public sealed record BiblePassageDto(
    string BibleId,
    string Reference,
    string Content,
    string Copyright);