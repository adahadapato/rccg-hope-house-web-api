using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Completes a pending two-factor authentication challenge.
/// </summary>
public sealed record CompleteTwoFactorLoginCommand(
    string ChallengeToken,
    string VerificationCode,
    bool UseRecoveryCode)
    : IRequest<AuthTokensDto>;