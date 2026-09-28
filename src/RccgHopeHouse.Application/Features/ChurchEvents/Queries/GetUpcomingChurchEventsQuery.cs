using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

/// <summary>
/// Retrieves active current and upcoming church
/// events for the public Upcoming Events section.
/// </summary>
public record GetUpcomingChurchEventsQuery(
    int Take = 20)
    : IRequest<IReadOnlyList<ChurchEventFeedDto>>;