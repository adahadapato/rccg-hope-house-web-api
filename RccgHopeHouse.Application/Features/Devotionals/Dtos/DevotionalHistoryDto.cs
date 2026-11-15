using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.Devotionals.Dtos;

// ============================================================
// Public history DTO — lightweight devotional archive
// ============================================================

/// <summary>
/// Lightweight Data Transfer Object for published devotional
/// history/archive listings.
/// </summary>
public record DevotionalHistoryDto(
    Guid Id,
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference)
{
    public static DevotionalHistoryDto FromEntity(
        Devotional devotional) => new(
        Id: devotional.Id,
        DevotionalDate: devotional.DevotionalDate,
        Theme: devotional.Theme,
        ScriptureReference: devotional.ScriptureReference);
}