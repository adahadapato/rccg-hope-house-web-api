using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

/// <summary>
/// Gets the complete content of the latest
/// published prophecy year.
/// </summary>
public record GetLatestPropheciesQuery()
    : IRequest<ProphecyYearDetailDto>;
