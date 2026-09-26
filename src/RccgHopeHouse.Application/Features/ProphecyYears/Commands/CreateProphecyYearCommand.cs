using MediatR;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Commands;

/// <summary>
/// Creates a new prophecy year.
/// </summary>
public record CreateProphecyYearCommand(
    int Year)
    : IRequest<ProphecyYearDto>;
