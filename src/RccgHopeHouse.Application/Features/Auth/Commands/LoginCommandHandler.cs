using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Handles user login, validates credentials via IAuthService,
/// and maps the authentication result to AuthTokensDto.
/// </summary>
public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthTokensDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates the user and returns JWT tokens
    /// together with authenticated user information.
    /// Throws UnauthorizedAccessException on failure.
    /// </summary>
    public async Task<AuthTokensDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.IsSuccess)
        {
            throw new UnauthorizedAccessException(
                result.ErrorMessage ??
                "Invalid email or password.");
        }

        // Map Core AuthResult → Application DTO
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