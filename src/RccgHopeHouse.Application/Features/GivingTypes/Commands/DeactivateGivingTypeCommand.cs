using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Command used to deactivate an existing giving type.
/// </summary>
/// <param name="Id">
/// The unique identifier of the giving type to deactivate.
/// </param>
public record DeactivateGivingTypeCommand(
    Guid Id) : IRequest<GivingTypeDto>;