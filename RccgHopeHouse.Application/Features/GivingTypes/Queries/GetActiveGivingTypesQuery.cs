using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Query used to retrieve giving types that are currently active
/// and available for selection on the Give Online form.
/// </summary>
public record GetActiveGivingTypesQuery()
    : IRequest<IReadOnlyList<GivingTypeDto>>;