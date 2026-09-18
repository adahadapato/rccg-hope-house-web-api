using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Queries;

/// <summary>
/// Gets the church's public contact/profile info, including a live active
/// member count. Public, unauthenticated — powers the Contact page.
/// </summary>
public record GetChurchInfoQuery : IRequest<ChurchInfoDto>;