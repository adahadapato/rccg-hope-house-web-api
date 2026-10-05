using MediatR;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Requests synchronization of service broadcasts with videos discovered
/// from the configured YouTube channels.
/// </summary>
public sealed record SynchronizeServiceBroadcastsCommand
    : IRequest<ServiceBroadcastSynchronizationResult>;
