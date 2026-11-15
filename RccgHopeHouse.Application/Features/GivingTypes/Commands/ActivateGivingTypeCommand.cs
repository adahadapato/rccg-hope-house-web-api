using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Command used to activate an existing giving type.
/// </summary>
/// <param name="Id">
/// The unique identifier of the giving type to activate.
/// </param>
public record ActivateGivingTypeCommand(
    Guid Id) : IRequest<GivingTypeDto>;