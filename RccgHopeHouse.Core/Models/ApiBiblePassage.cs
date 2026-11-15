namespace RccgHopeHouse.Core.Models;

/// <summary>
/// Represents a Bible passage retrieved from the external API.Bible service.
/// </summary>
/// <param name="BibleId">
/// API.Bible's unique identifier for the Bible resource used to retrieve
/// the passage.
/// </param>
/// <param name="Reference">
/// Human-readable scripture reference returned by API.Bible.
/// </param>
/// <param name="Content">
/// Scripture text returned by API.Bible.
/// </param>
/// <param name="Copyright">
/// Copyright information supplied by the Bible provider.
/// </param>
public sealed record ApiBiblePassage(
    string BibleId,
    string Reference,
    string Content,
    string Copyright);