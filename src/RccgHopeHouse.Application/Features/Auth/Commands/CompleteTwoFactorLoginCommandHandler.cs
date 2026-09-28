using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Verifies the second authentication factor and,
/// when valid, returns the final JWT access and refresh tokens.
/// </summary>
public sealed class CompleteTwoFactorLoginCommandHandler
    : IRequestHandler<
        CompleteTwoFactorLoginCommand,
        AuthTokensDto>
{
    private readonly IAuthService _authService;

    public CompleteTwoFactorLoginCommandHandler(
        IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthTokensDto> Handle(
        CompleteTwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        var result =
            await _authService.CompleteTwoFactorLoginAsync(
                request.ChallengeToken,
                request.VerificationCode,
                request.UseRecoveryCode,
                cancellationToken);

        if (!result.IsSuccess)
        {
            throw new UnauthorizedAccessException(
                result.ErrorMessage ??
                "Two-factor authentication failed.");
        }

        return new AuthTokensDto(
            result.AccessToken ?? string.Empty,
            result.RefreshToken ?? string.Empty,
            result.ExpiresAt ?? DateTime.MinValue,
            result.Role ?? string.Empty,
            result.UserName ?? string.Empty,
            result.Name ?? string.Empty,
            result.Email ?? string.Empty,
            RequiresTwoFactor: false,
            TwoFactorChallengeToken: null);
    }
}