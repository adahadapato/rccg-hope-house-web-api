using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Provides canonical Bible reference information used for scripture
/// selection, validation, and passage identifier generation.
/// </summary>
public interface IBibleReferenceService
{
    /// <summary>
    /// Returns all canonical Bible books together with their
    /// chapter and verse-count structure.
    /// </summary>
    IReadOnlyList<BibleBookReference> GetBooks();

    /// <summary>
    /// Finds a Bible book using its canonical API.Bible/USFM book code.
    /// </summary>
    /// <param name="bookCode">
    /// The canonical book code, for example GEN, EXO, PSA, or JHN.
    /// </param>
    /// <returns>
    /// The matching Bible book, or null when the supplied code
    /// does not identify a supported book.
    /// </returns>
    BibleBookReference? GetBook(string bookCode);

    /// <summary>
    /// Determines whether a chapter and verse range is valid
    /// for the specified Bible book.
    /// </summary>
    /// <param name="bookCode">
    /// The canonical API.Bible/USFM book code.
    /// </param>
    /// <param name="chapter">
    /// The chapter number.
    /// </param>
    /// <param name="startVerse">
    /// The first verse in the passage.
    /// </param>
    /// <param name="endVerse">
    /// The final verse in the passage.
    /// </param>
    bool IsValidReference(
        string bookCode,
        int chapter,
        int startVerse,
        int endVerse);

    /// <summary>
    /// Creates a human-readable scripture reference from a validated
    /// Bible book, chapter, and verse range.
    /// </summary>
    /// <example>
    /// EXO, 14, 1, 4 becomes Exodus 14:1-4.
    /// </example>
    string BuildScriptureReference(
        string bookCode,
        int chapter,
        int startVerse,
        int endVerse);

    /// <summary>
    /// Creates the passage identifier required by API.Bible.
    /// </summary>
    /// <example>
    /// EXO, 14, 1, 4 becomes EXO.14.1-EXO.14.4.
    /// </example>
    string BuildPassageId(
        string bookCode,
        int chapter,
        int startVerse,
        int endVerse);
}