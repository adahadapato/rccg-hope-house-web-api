using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

/// <summary>
/// Represents service broadcast information returned by the application.
/// </summary>
public sealed record ServiceBroadcastDto(
    Guid Id,
    Guid ChurchServiceId,
    ServiceCategory Category,
    string Title,
    string VideoId,
    string VideoUrl,
    string ThumbnailUrl,
    string? Description,
    string? Theme,
    DateTime ServiceMonth,
    bool IsLive,
    bool IsPublished)
{
    /// <summary>
    /// Creates a service broadcast DTO from a domain entity.
    /// </summary>
    /// <param name="broadcast">
    /// The service broadcast to map.
    /// </param>
    /// <returns>
    /// A DTO containing the broadcast information.
    /// </returns>
    public static ServiceBroadcastDto FromEntity(
        ServiceBroadcast broadcast)
    {
        return new ServiceBroadcastDto(
            Id: broadcast.Id,
            ChurchServiceId: broadcast.ChurchServiceId,
            Category: broadcast.Category,
            Title: broadcast.Title,
            VideoId: broadcast.VideoId,
            VideoUrl: broadcast.VideoUrl,
            ThumbnailUrl: broadcast.ThumbnailUrl,
            Description: broadcast.Description,
            Theme: broadcast.Theme,
            ServiceMonth: broadcast.ServiceMonth,
            IsLive: broadcast.IsLive,
            IsPublished: broadcast.IsPublished);
    }
}