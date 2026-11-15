using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public record PublishProphecyYearCommand(
 Guid Id)
 : IRequest<ProphecyYearDto>;
