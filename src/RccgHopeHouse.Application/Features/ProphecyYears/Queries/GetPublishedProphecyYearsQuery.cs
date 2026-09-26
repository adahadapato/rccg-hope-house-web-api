using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

/// <summary>
/// Gets all prophecy years currently available to the public.
/// </summary>
public record GetPublishedProphecyYearsQuery()
    : IRequest<IReadOnlyList<ProphecyYearDto>>;
