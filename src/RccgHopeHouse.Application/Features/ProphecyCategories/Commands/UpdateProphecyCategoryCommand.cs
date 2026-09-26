using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public record UpdateProphecyCategoryCommand(
 Guid Id,
 string Name,
 string? Description,
 int DisplayOrder)
 : IRequest<ProphecyCategoryDto>;
