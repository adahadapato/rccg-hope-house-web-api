namespace RccgHopeHouse.Core.Models;

/// <summary>
/// Represents a canonical Bible book and its chapter structure
/// for scripture-reference selection and validation.
/// </summary>
/// <param name="Name">
/// Canonical display name of the Bible book.
/// </param>
/// <param name="Code">
/// API.Bible book code used when constructing passage identifiers.
/// </param>
/// <param name="ChapterVerseCounts">
/// Number of verses in each chapter.
/// The first value represents chapter 1, the second chapter 2, and so on.
/// </param>
public sealed record BibleBookReference(
    string Name,
    string Code,
    IReadOnlyList<int> ChapterVerseCounts);