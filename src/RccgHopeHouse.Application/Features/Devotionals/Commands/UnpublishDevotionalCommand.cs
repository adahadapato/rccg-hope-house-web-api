using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Command to unpublish a devotional.
/// An unpublished devotional is not publicly accessible.
/// </summary>
public record UnpublishDevotionalCommand(
    Guid Id) : IRequest<DevotionalDto>;