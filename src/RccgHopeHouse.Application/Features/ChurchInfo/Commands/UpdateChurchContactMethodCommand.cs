using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Represents a request to update an existing church contact method.
/// </summary>
public record UpdateChurchContactMethodCommand(
    Guid Id,
    ContactMethodType Type,
    string Value,
    string? Label,
    int DisplayOrder) : IRequest<ChurchInfoDto>;