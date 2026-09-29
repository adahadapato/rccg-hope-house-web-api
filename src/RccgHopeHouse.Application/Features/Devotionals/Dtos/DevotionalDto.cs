using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.Devotionals.Dtos;

// ============================================================
// Full DTO — public devotional views and admin management
// ============================================================

/// <summary>
/// Data Transfer Object for a daily devotional.
/// Used for public devotional views, devotional history,
/// admin management, and API responses.
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
    DateTime? PublishedAt)
{
    public static DevotionalDto FromEntity(Devotional devotional) => new(
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
        PublishedAt: devotional.PublishedAt);
}
