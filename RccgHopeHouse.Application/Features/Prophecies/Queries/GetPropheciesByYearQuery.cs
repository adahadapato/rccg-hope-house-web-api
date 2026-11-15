using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

/// <summary>
/// Gets the complete published prophecy content
/// for a specified year.
/// </summary>
public record GetPropheciesByYearQuery(
    int Year)
    : IRequest<ProphecyYearDetailDto>;
