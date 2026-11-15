using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Queries;

/// <summary>
/// Query used to retrieve a giving type by its unique identifier.
/// </summary>
public record GetGivingTypeByIdQuery(
    Guid Id) : IRequest<GivingTypeDto>;