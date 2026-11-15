using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Queries;

/// <summary>
/// Gets a prophecy year by its unique identifier.
/// </summary>
public record GetProphecyYearByIdQuery(
    Guid Id)
    : IRequest<ProphecyYearDto>;