
using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Updates devotional content without changing publication state.
/// Null optional values preserve existing additional content.
/// </summary>
public record UpdateDevotionalCommand(
    Guid Id,
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference,
    string PassageId,
    string Thought,
    IReadOnlyList<string> CommentaryPoints,
    IReadOnlyList<string> PrayerPoints,
    string Declaration,
    string? MemoryVerseReference = null,
    string? BibleInOneYearReference = null,
    IReadOnlyList<string>? BibleInOneYearReferences = null,
    string? HymnNumber = null,
    string? HymnTitle = null,
    string? HymnLyrics = null,
    string? AdditionalReading = null,
    string? KeyPoint = null,
    string? Author = null,
    string? SourceUrl = null
) : IRequest<DevotionalDto>;
