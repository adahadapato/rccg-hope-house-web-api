using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Detailed DTO for admin views and single-service API responses.
/// Includes schedule, recurrence, Zoom configuration, presentation
/// metadata and visibility settings.
/// </summary>
public record ChurchServiceDto(
    Guid Id,
    string Name,
    ServiceCategory Category,
    DayOfWeek DayOfWeek,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Description,
    string? Location,
    string? ZoomId,
    string? ZoomPasscode,
    RecurrencePattern Recurrence,
    int? DayOfMonth,
    bool IsLocal,
    bool IsActive,
    int DisplayOrder,
    string? Icon,
    bool ShowInMonthlyServices);