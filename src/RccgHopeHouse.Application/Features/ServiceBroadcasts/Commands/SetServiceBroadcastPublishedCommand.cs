using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Changes the public publication state of an individual
/// service broadcast.
/// </summary>
/// <param name="Id">
/// The unique identifier of the service broadcast.
/// </param>
/// <param name="IsPublished">
/// <see langword="true"/> to show the broadcast on the public website;
/// otherwise <see langword="false"/> to hide it.
/// </param>
public sealed record SetServiceBroadcastPublishedCommand(
    Guid Id,
    bool IsPublished)
    : IRequest<ServiceBroadcastDto>;