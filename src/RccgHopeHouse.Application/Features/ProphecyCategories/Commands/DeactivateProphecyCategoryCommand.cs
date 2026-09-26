using MediatR;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

public record DeactivateProphecyCategoryCommand(
 Guid Id)
 : IRequest<ProphecyCategoryDto>;
