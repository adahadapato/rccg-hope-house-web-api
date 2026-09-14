using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Detailed DTO for admin views and single-service API responses.
/// Includes all metadata, recurrence rules, and Zoom configuration.
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
    bool IsActive,
    int DisplayOrder);