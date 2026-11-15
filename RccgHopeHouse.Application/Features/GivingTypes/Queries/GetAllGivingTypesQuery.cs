using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Query used to retrieve all giving types,
/// including inactive giving types.
/// </summary>
public record GetAllGivingTypesQuery()
    : IRequest<IReadOnlyList<GivingTypeDto>>;