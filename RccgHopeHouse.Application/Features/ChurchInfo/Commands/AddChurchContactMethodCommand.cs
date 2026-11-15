using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Represents a request to add a contact method to the church profile.
/// </summary>
public record AddChurchContactMethodCommand(
    ContactMethodType Type,
    string Value,
    string? Label,
    int DisplayOrder) : IRequest<ChurchInfoDto>;