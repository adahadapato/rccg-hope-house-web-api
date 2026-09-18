using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

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
    bool IsLocal = true,
    int DisplayOrder = 0) : IRequest<ChurchServiceDto>;