using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Detailed DTO for admin views and single-service API responses.
/// Includes schedule, recurrence, Zoom configuration, presentation
/// metadata, visibility settings, broadcast configuration and the
/// theme for the upcoming or currently occurring service.
/// </summary>
/// <param name="Id">
/// The unique identifier of the church service.
/// </param>
/// <param name="Name">
/// The name of the church service.
/// </param>
/// <param name="Category">
/// The category to which the service belongs.
/// </param>
/// <param name="DayOfWeek">
/// The day of the week on which the service is normally held.
/// </param>
/// <param name="StartTime">
/// The scheduled start time of the service, if specified.
/// </param>
/// <param name="EndTime">
/// The scheduled end time of the service, if specified.
/// </param>
/// <param name="Description">
/// An optional description of the service.
/// </param>
/// <param name="Location">
/// The physical location of the service, if applicable.
/// </param>
/// <param name="ZoomId">
/// The Zoom meeting identifier, if applicable.
/// </param>
/// <param name="ZoomPasscode">
/// The Zoom meeting passcode, if applicable.
/// </param>
/// <param name="Recurrence">
/// Defines how frequently the service occurs.
/// </param>
/// <param name="DayOfMonth">
/// The configured day of the month for monthly services,
/// when applicable.
/// </param>
/// <param name="IsLocal">
/// Indicates whether the service belongs to Hope House locally.
/// </param>
/// <param name="IsActive">
/// Indicates whether the church service is currently active.
/// </param>
/// <param name="DisplayOrder">
/// Determines the display position of the service in ordered listings.
/// </param>
/// <param name="Icon">
/// An optional icon displayed for the service.
/// </param>
/// <param name="ShowInMonthlyServices">
/// Indicates whether the service should appear in the public
/// Special Monthly Services section.
/// </param>
/// <param name="IsBroadcastEnabled">
/// Indicates whether video broadcasts may be associated with
/// the church service.
/// </param>
/// <param name="CurrentTheme">
/// The theme for the upcoming or currently occurring instance
/// of the church service. This is independent of historical
/// themes stored against individual service broadcasts.
/// </param>
public sealed record ChurchServiceDto(
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
    bool ShowInMonthlyServices,
    bool IsBroadcastEnabled,
    string? CurrentTheme);