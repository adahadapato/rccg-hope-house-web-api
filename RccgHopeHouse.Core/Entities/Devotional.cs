
using System.Text.Json;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a daily devotional, including scripture readings,
/// memory verses, annual Bible readings, hymns and publication details.
/// </summary>
public class Devotional : BaseEntity
{
    public DateOnly DevotionalDate { get; private set; }

    public string Theme { get; private set; } = string.Empty;

    public string ScriptureReference { get; private set; } = string.Empty;

    public string PassageId { get; private set; } = string.Empty;

    public string MemoryVerseReference { get; private set; } = string.Empty;

    public string MemoryVersePassageId { get; private set; } = string.Empty;

    public string BibleInOneYearReference { get; private set; } = string.Empty;

    /// <summary>
    /// JSON array of validated Bible passage IDs.
    /// </summary>
    public string BibleInOneYearPassageIdsJson { get; private set; } = "[]";

    public string Thought { get; private set; } = string.Empty;

    public string CommentaryJson { get; private set; } = "[]";

    public string PrayerPointsJson { get; private set; } = "[]";

    public string Declaration { get; private set; } = string.Empty;

    public string? HymnNumber { get; private set; } = string.Empty;

    public string? HymnTitle { get; private set; } = string.Empty;

    public string? HymnLyrics { get; private set; } = string.Empty;

    public string AdditionalReading { get; private set; } = string.Empty;

    public string KeyPoint { get; private set; } = string.Empty;

    public string Author { get; private set; } = string.Empty;

    public string SourceUrl { get; private set; } = string.Empty;

    public bool IsPublished { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    private Devotional()
    {
    }

    /// <summary>
    /// Creates a devotional as an unpublished draft.
    /// </summary>
    public static Devotional Create(
        DateOnly devotionalDate,
        string theme,
        string scriptureReference,
        string passageId,
        string thought,
        IEnumerable<string> commentaryPoints,
        IEnumerable<string> prayerPoints,
        string declaration,
        string memoryVerseReference = "",
        string memoryVersePassageId = "",
        string bibleInOneYearReference = "",
        IEnumerable<string>? bibleInOneYearPassageIds = null,
        string? hymnNumber = "",
        string? hymnTitle = "",
        string? hymnLyrics = "",
        string additionalReading = "",
        string keyPoint = "",
        string author = "",
        string sourceUrl = "")
    {
        Validate(
            devotionalDate,
            theme,
            scriptureReference,
            passageId,
            thought,
            commentaryPoints,
            prayerPoints,
            declaration);

        var devotional = new Devotional
        {
            DevotionalDate = devotionalDate,
            Theme = theme.Trim(),
            ScriptureReference = scriptureReference.Trim(),
            PassageId = passageId.Trim(),
            Thought = thought.Trim(),
            CommentaryJson = JsonSerializer.Serialize(
                CleanPoints(commentaryPoints, nameof(commentaryPoints))),
            PrayerPointsJson = JsonSerializer.Serialize(
                CleanPoints(prayerPoints, nameof(prayerPoints))),
            Declaration = declaration.Trim(),
            IsPublished = false,
            PublishedAt = null
        };

        devotional.SetAdditionalContent(
            memoryVerseReference,
            memoryVersePassageId,
            bibleInOneYearReference,
            bibleInOneYearPassageIds,
            hymnNumber,
            hymnTitle,
            hymnLyrics,
            additionalReading,
            keyPoint,
            author,
            sourceUrl);

        return devotional;
    }

    /// <summary>
    /// Updates devotional content without changing publication state.
    /// </summary>
    public void Update(
        DateOnly devotionalDate,
        string theme,
        string scriptureReference,
        string passageId,
        string thought,
        IEnumerable<string> commentaryPoints,
        IEnumerable<string> prayerPoints,
        string declaration,
        string? memoryVerseReference = null,
        string? memoryVersePassageId = null,
        string? bibleInOneYearReference = null,
        IEnumerable<string>? bibleInOneYearPassageIds = null,
        string? hymnNumber = null,
        string? hymnTitle = null,
        string? hymnLyrics = null,
        string? additionalReading = null,
        string? keyPoint = null,
        string? author = null,
        string? sourceUrl = null)
    {
        Validate(
            devotionalDate,
            theme,
            scriptureReference,
            passageId,
            thought,
            commentaryPoints,
            prayerPoints,
            declaration);

        DevotionalDate = devotionalDate;
        Theme = theme.Trim();
        ScriptureReference = scriptureReference.Trim();
        PassageId = passageId.Trim();
        Thought = thought.Trim();

        CommentaryJson = JsonSerializer.Serialize(
            CleanPoints(commentaryPoints, nameof(commentaryPoints)));

        PrayerPointsJson = JsonSerializer.Serialize(
            CleanPoints(prayerPoints, nameof(prayerPoints)));

        Declaration = declaration.Trim();

        // Null means an older client did not submit these fields.
        // Empty strings explicitly clear optional content.
        SetAdditionalContent(
            memoryVerseReference ?? MemoryVerseReference,
            memoryVersePassageId ?? MemoryVersePassageId,
            bibleInOneYearReference ?? BibleInOneYearReference,
            bibleInOneYearPassageIds ?? GetBibleInOneYearPassageIds(),
            hymnNumber ?? HymnNumber,
            hymnTitle ?? HymnTitle,
            hymnLyrics ?? HymnLyrics,
            additionalReading ?? AdditionalReading,
            keyPoint ?? KeyPoint,
            author ?? Author,
            sourceUrl ?? SourceUrl);

        MarkAsUpdated();
    }

    /// <summary>
    /// Stores additional devotional content and passage IDs.
    /// Passage IDs must be generated and validated by the
    /// application service before this method is called.
    /// </summary>
    private void SetAdditionalContent(
        string memoryVerseReference,
        string memoryVersePassageId,
        string bibleInOneYearReference,
        IEnumerable<string>? bibleInOneYearPassageIds,
        string? hymnNumber,
        string? hymnTitle,
        string? hymnLyrics,
        string additionalReading,
        string keyPoint,
        string author,
        string sourceUrl)
    {
        MemoryVerseReference = memoryVerseReference.Trim();
        MemoryVersePassageId = memoryVersePassageId.Trim();
        BibleInOneYearReference = bibleInOneYearReference.Trim();

        var passageIds = (bibleInOneYearPassageIds ?? [])
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        BibleInOneYearPassageIdsJson = JsonSerializer.Serialize(passageIds);
        HymnNumber = string.IsNullOrWhiteSpace(hymnNumber) ? null : hymnNumber.Trim();
        HymnTitle = string.IsNullOrWhiteSpace(hymnTitle) ? null : hymnTitle.Trim();
        HymnLyrics = string.IsNullOrWhiteSpace(hymnLyrics) ? null : hymnLyrics.Trim();
        AdditionalReading = additionalReading.Trim();
        KeyPoint = keyPoint.Trim();
        Author = author.Trim();
        SourceUrl = sourceUrl.Trim();
    }

    public void Publish()
    {
        if (IsPublished)
            return;

        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Unpublish()
    {
        if (!IsPublished)
            return;

        IsPublished = false;
        MarkAsUpdated();
    }

    public IReadOnlyList<string> GetCommentaryPoints() =>
        DeserializeList(CommentaryJson);

    public IReadOnlyList<string> GetPrayerPoints() =>
        DeserializeList(PrayerPointsJson);

    public IReadOnlyList<string> GetBibleInOneYearPassageIds() =>
        DeserializeList(BibleInOneYearPassageIdsJson);

    private static IReadOnlyList<string> DeserializeList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static List<string> CleanPoints(
        IEnumerable<string> points,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(points);

        var cleaned = points
            .Where(point => !string.IsNullOrWhiteSpace(point))
            .Select(point => point.Trim())
            .ToList();

        if (cleaned.Count == 0)
        {
            throw new ArgumentException(
                "At least one point is required.",
                parameterName);
        }

        return cleaned;
    }

    private static void Validate(
        DateOnly devotionalDate,
        string theme,
        string scriptureReference,
        string passageId,
        string thought,
        IEnumerable<string> commentaryPoints,
        IEnumerable<string> prayerPoints,
        string declaration)
    {
        if (devotionalDate == default)
        {
            throw new ArgumentException(
                "Devotional date is required.",
                nameof(devotionalDate));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(theme);
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptureReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(passageId);
        ArgumentException.ThrowIfNullOrWhiteSpace(thought);
        ArgumentException.ThrowIfNullOrWhiteSpace(declaration);

        _ = CleanPoints(commentaryPoints, nameof(commentaryPoints));
        _ = CleanPoints(prayerPoints, nameof(prayerPoints));
    }
}
