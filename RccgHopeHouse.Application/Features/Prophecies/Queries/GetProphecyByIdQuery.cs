using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Queries;

/// <summary>
/// Gets an individual prophecy by its unique identifier.
/// </summary>
public record GetProphecyByIdQuery(
    Guid Id)
    : IRequest<ProphecyDto>;