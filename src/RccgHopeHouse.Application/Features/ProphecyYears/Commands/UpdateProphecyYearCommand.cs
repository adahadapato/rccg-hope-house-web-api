using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public record UpdateProphecyYearCommand(
 Guid Id,
 int Year)
 : IRequest<ProphecyYearDto>;
