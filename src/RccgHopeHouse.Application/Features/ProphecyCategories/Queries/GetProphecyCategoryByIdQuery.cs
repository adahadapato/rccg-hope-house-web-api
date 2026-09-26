using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Queries;

/// <summary>
/// Gets a prophecy category by its unique identifier.
/// </summary>
public record GetProphecyCategoryByIdQuery(
    Guid Id)
    : IRequest<ProphecyCategoryDto>;
