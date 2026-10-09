
namespace RccgHopeHouse.Core.Results.AI;

/// <summary>
/// Represents structured Open Heavens devotional content
/// retrieved and processed by the AI service.
///
/// Bible passage IDs are not supplied by AI.
/// They are generated and validated by the application
/// using the existing Bible reference service.
/// </summary>
public sealed record OpenHeavensDevotionalResult(
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference,
    string Thought,
    IReadOnlyList<string> CommentaryPoints,
    IReadOnlyList<string> PrayerPoints,
    string Declaration,
    string SourceUrl,
    string MemoryVerseReference = "",
    string BibleInOneYearReference = "",
    IReadOnlyList<string>? BibleInOneYearReferences = null,
    string HymnNumber = "",
    string HymnTitle = "",
    string HymnLyrics = "",
    string AdditionalReading = "",
    string KeyPoint = "",
    string Author = "");
