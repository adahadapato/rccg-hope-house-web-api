using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Command to update an existing daily devotional.
/// Publication state is managed separately.
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
    string Declaration) : IRequest<DevotionalDto>;