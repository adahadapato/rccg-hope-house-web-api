using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Public DTO for church service schedule feeds.
/// Contains the scheduling and presentation information required
/// for the website to render services without hard-coded mappings.
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
    bool IsLocal,
    bool IsActive,
    RecurrencePattern Recurrence,
    int? DayOfMonth,
    int DisplayOrder,
    string? Icon,
    bool ShowInMonthlyServices);