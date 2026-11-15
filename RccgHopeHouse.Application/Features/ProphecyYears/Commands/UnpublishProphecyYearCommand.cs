using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

public record UnpublishProphecyYearCommand(
 Guid Id)
 : IRequest<ProphecyYearDto>;
