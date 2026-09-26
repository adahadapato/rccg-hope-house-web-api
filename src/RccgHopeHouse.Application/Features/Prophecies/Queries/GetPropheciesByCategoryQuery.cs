using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

/// <summary>
/// Gets prophecy statements belonging to a specified category.
/// </summary>
public record GetPropheciesByCategoryQuery(
    Guid CategoryId,
    bool ActiveOnly = false)
    : IRequest<IReadOnlyList<ProphecyDto>>;