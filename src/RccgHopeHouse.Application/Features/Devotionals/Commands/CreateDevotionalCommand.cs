
using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Creates a new devotional as a draft.
/// Passage IDs for additional readings are generated automatically.
/// </summary>
public record CreateDevotionalCommand(
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
