using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Handles refresh token requests and returns a new
/// access/refresh token pair together with user information.
/// </summary>
public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(
        IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthTokensDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.RefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

        if (!result.IsSuccess)
        {
            throw new UnauthorizedAccessException(
                result.ErrorMessage ??
                "Unable to refresh authentication token.");
        }

        return new AuthTokensDto(
            result.AccessToken ?? string.Empty,
            result.RefreshToken ?? string.Empty,
            result.ExpiresAt ?? DateTime.MinValue,
            result.Role ?? string.Empty,
            result.UserName ?? string.Empty,
            result.Name ?? string.Empty,
            result.Email ?? string.Empty);
    }
}