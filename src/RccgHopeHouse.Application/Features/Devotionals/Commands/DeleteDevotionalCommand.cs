using MediatR;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Command to delete a devotional.
/// The devotional is soft-deleted rather than physically removed.
/// </summary>
public record DeleteDevotionalCommand(
    Guid Id) : IRequest;