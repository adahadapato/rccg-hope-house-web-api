using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Represents a request to remove a contact method from the church profile.
/// </summary>
public record RemoveChurchContactMethodCommand(
    Guid Id) : IRequest<ChurchInfoDto>;