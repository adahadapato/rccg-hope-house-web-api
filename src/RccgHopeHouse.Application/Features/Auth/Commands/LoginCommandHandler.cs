using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Handles user login, validates credentials via IAuthService, and maps to AuthTokensDto.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokensDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates the user and returns JWT tokens.
    /// Throws UnauthorizedAccessException on failure.
    /// </summary>
    public async Task<AuthTokensDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (!result.IsSuccess)
            throw new UnauthorizedAccessException(result.ErrorMessage ?? "Invalid email or password.");

        // Map Core AuthResult → Application DTO
        return new AuthTokensDto(
            result.AccessToken ?? string.Empty,
            result.RefreshToken ?? string.Empty,
            result.ExpiresAt ?? DateTime.MinValue,
            result.Role ?? string.Empty,
            result.UserName ?? string.Empty);
    }
}