using MediatR;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public record DeleteServiceBroadcastCommand(Guid Id) : IRequest<Unit>;