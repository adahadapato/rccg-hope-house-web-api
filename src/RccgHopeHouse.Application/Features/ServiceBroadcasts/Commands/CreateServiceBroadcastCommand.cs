using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Command to record this month's broadcast video for a service (Holy
/// Communion, Holy Ghost Service, or Thanksgiving Service). Admin pastes
/// a YouTube URL; the video ID and thumbnail are derived automatically.
/// </summary>
public record CreateServiceBroadcastCommand(
    ServiceCategory Category,
    string Title,
    string YoutubeUrl,
    DateTime ServiceMonth,
    string? Description = null,
    string? Theme = null,
    bool IsLive = false) : IRequest<ServiceBroadcastDto>;