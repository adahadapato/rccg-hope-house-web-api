using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Records a monthly broadcast against a specific church service.
/// The service itself determines the broadcast category.
/// </summary>
public record CreateServiceBroadcastCommand(
    Guid ChurchServiceId,
    string Title,
    string YoutubeUrl,
    DateTime ServiceMonth,
    string? Description = null,
    string? Theme = null,
    bool IsLive = false
) : IRequest<ServiceBroadcastDto>;