using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

/// <summary>
/// Gets all prophecy years for administration,
/// including unpublished years.
/// </summary>
public record GetAllProphecyYearsQuery()
    : IRequest<IReadOnlyList<ProphecyYearDto>>;