using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Command used to update an existing giving type.
/// </summary>
/// <param name="Id">
/// The unique identifier of the giving type to update.
/// </param>
/// <param name="Name">
/// The updated display name.
/// </param>
/// <param name="Description">
/// The updated optional description.
/// </param>
/// <param name="DisplayOrder">
/// The updated display position.
/// </param>
public record UpdateGivingTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<GivingTypeDto>;