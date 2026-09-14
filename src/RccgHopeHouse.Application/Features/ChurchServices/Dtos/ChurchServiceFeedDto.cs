using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Lightweight DTO for public schedule feeds.
/// Excludes administrative fields (Description, DayOfMonth, DisplayOrder) for faster payload delivery.
/// </summary>
public record ChurchServiceFeedDto(
    Guid Id,
    string Name,
    ServiceCategory Category,
    DayOfWeek DayOfWeek,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Location,
    string? ZoomId,
    string? ZoomPasscode,
    bool IsActive);