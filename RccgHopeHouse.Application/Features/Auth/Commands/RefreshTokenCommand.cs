using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Command to exchange a valid refresh token for a new access/refresh token pair.
/// Used when the access token expires but the user session is still active.
/// </summary>
public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthTokensDto>;