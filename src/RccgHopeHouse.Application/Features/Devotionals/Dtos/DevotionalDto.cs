
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.Devotionals.Dtos;

/// <summary>
/// Full devotional DTO for public and administrative use.
/// Includes automatically generated Bible passage IDs.
/// </summary>
public record DevotionalDto(
    Guid Id,
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference,
    string PassageId,
    string Thought,
    IReadOnlyList<string> CommentaryPoints,
    IReadOnlyList<string> PrayerPoints,
    string Declaration,
    bool IsPublished,
    DateTime? PublishedAt,
    string MemoryVerseReference,
    string MemoryVersePassageId,
    string BibleInOneYearReference,
    IReadOnlyList<string> BibleInOneYearPassageIds,
    string? HymnNumber,
    string? HymnTitle,
    string? HymnLyrics,
    string AdditionalReading,
    string KeyPoint,
    string Author,
    string? SourceUrl)
{
    /// <summary>
    /// Converts a devotional entity into a full API response.
    /// </summary>
    public static DevotionalDto FromEntity(
        Devotional devotional) => new(
            Id: devotional.Id,
            DevotionalDate: devotional.DevotionalDate,
            Theme: devotional.Theme,
            ScriptureReference: devotional.ScriptureReference,
            PassageId: devotional.PassageId,
            Thought: devotional.Thought,
            CommentaryPoints: devotional.GetCommentaryPoints(),
            PrayerPoints: devotional.GetPrayerPoints(),
            Declaration: devotional.Declaration,
            IsPublished: devotional.IsPublished,
            PublishedAt: devotional.PublishedAt,
            MemoryVerseReference: devotional.MemoryVerseReference,
            MemoryVersePassageId: devotional.MemoryVersePassageId,
            BibleInOneYearReference: devotional.BibleInOneYearReference,
            BibleInOneYearPassageIds: devotional.GetBibleInOneYearPassageIds(),
            HymnNumber: devotional.HymnNumber,
            HymnTitle: devotional.HymnTitle,
            HymnLyrics: devotional.HymnLyrics,
            AdditionalReading: devotional.AdditionalReading,
            KeyPoint: devotional.KeyPoint,
            Author: devotional.Author,
            SourceUrl: devotional.SourceUrl);
}
