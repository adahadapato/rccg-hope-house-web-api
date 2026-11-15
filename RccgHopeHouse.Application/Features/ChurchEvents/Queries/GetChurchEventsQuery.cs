using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

/// <summary>
/// Query to retrieve church events for
/// administrative management.
/// </summary>
public record GetChurchEventsQuery(
    ServiceCategory? Category = null,
    bool? IsActive = null,
    int Skip = 0,
    int Take = 100)
    : IRequest<IReadOnlyList<ChurchEventDto>>;