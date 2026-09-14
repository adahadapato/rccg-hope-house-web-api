using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Command to create a new church service schedule.
/// Created as active by default; can be deactivated later.
/// </summary>
public record CreateChurchServiceCommand(
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
    int DisplayOrder = 0) : IRequest<ChurchServiceDto>;