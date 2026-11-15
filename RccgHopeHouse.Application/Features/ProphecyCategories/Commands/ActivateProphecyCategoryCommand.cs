using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public record ActivateProphecyCategoryCommand(
 Guid Id)
 : IRequest<ProphecyCategoryDto>;
