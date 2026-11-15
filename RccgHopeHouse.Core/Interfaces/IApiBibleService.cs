using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines Bible operations provided through the external API.Bible integration.
/// </summary>
public interface IApiBibleService
{
    /// <summary>
    /// Gets the English Bible translations available to the configured
    /// API.Bible account.
    /// </summary>
    Task<IReadOnlyList<ApiBibleTranslation>> GetAvailableBiblesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific Bible passage from a specified API.Bible translation.
    /// </summary>
    /// <param name="bibleId">
    /// API.Bible identifier of the Bible translation.
    /// </param>
    /// <param name="passageId">
    /// API.Bible/USFM passage identifier, for example
    /// EXO.14.1-EXO.14.4.
    /// </param>
    Task<ApiBiblePassage> GetPassageAsync(
        string bibleId,
        string passageId,
        CancellationToken cancellationToken = default);
}