using MediatR;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;

namespace RccgHopeHouse.Application.Features.GivingTypes.Commands;

/// <summary>
/// Command used to create a new giving type.
/// </summary>
/// <param name="Name">
/// The display name of the giving type, for example
/// "Tithe", "Offering" or "Seed Offering".
/// </param>
/// <param name="Description">
/// Optional description explaining the purpose of the giving type.
/// </param>
/// <param name="DisplayOrder">
/// Determines the position of the giving type in displayed lists.
/// </param>
public record CreateGivingTypeCommand(
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<GivingTypeDto>;