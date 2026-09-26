using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Queries;

/// <summary>
/// Gets prophecy categories belonging to a specified prophecy year.
/// </summary>
public record GetProphecyCategoriesByYearQuery(
    Guid ProphecyYearId,
    bool ActiveOnly = false)
    : IRequest<IReadOnlyList<ProphecyCategoryDto>>;
