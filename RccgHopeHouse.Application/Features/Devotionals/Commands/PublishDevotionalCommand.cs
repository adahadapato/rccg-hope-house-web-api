using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Command to publish a devotional.
/// Publishing makes the devotional eligible for public display,
/// subject to its devotional date.
/// </summary>
public record PublishDevotionalCommand(
    Guid Id) : IRequest<DevotionalDto>;