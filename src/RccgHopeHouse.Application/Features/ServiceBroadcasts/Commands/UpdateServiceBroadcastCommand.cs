using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public record UpdateServiceBroadcastCommand(
    Guid Id,
    string Title,
    string YoutubeUrl,
    string? Description,
    string? Theme,
    bool IsLive) : IRequest<ServiceBroadcastDto>;