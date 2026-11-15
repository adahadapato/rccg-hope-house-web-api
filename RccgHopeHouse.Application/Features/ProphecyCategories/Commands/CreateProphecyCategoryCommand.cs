using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public record CreateProphecyCategoryCommand(
    Guid ProphecyYearId,
    string Name,
    string? Description,
    int DisplayOrder)
    : IRequest<ProphecyCategoryDto>;
