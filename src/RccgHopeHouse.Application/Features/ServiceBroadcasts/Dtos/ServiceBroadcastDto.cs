using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

public record ServiceBroadcastDto(
    Guid Id,
    ServiceCategory Category,
    string Title,
    string VideoId,
    string VideoUrl,
    string ThumbnailUrl,
    string? Description,
    string? Theme,
    DateTime ServiceMonth,
    bool IsLive)
{
    public static ServiceBroadcastDto FromEntity(ServiceBroadcast b) => new(
        Id: b.Id,
        Category: b.Category,
        Title: b.Title,
        VideoId: b.VideoId,
        VideoUrl: b.VideoUrl,
        ThumbnailUrl: b.ThumbnailUrl,
        Description: b.Description,
        Theme: b.Theme,
        ServiceMonth: b.ServiceMonth,
        IsLive: b.IsLive);
}