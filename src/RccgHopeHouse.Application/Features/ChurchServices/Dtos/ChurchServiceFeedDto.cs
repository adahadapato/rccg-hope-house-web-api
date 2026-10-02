using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Dtos;

/// <summary>
/// Public DTO for church service schedule feeds.
/// Contains the scheduling and presentation information required
/// for the website to render services without hard-coded mappings.
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
/// <param name="Location">
/// The physical location of the service, if applicable.
/// </param>
/// <param name="ZoomId">
/// The Zoom meeting identifier, if applicable.
/// </param>
/// <param name="ZoomPasscode">
/// The Zoom meeting passcode, if applicable.
/// </param>
/// <param name="IsLocal">
/// Indicates whether the service belongs to Hope House locally.
/// </param>
/// <param name="IsActive">
/// Indicates whether the church service is currently active.
/// </param>
/// <param name="Recurrence">
/// Defines how frequently the service occurs.
/// </param>
/// <param name="DayOfMonth">
/// The configured day of the month for monthly services,
/// when applicable.
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
/// Indicates whether video broadcasts are enabled for this service.
/// This allows the public website to determine whether the service
/// should participate in broadcast-related presentation.
/// </param>
/// <param name="CurrentTheme">
/// The theme for the upcoming or currently occurring instance
/// of the service. Historical broadcast themes remain associated
/// with their individual broadcast records.
/// </param>
public sealed record ChurchServiceFeedDto(
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
    bool ShowInMonthlyServices,
    bool IsBroadcastEnabled,
    string? CurrentTheme);