// Application/Features/Auth/Commands/RefreshTokenCommandHandler.cs
using MediatR;
using RccgHopeHouse.Application.Features.Auth.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService) => _authService = authService;

    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Call Core interface
        var coreResult = await _authService.RefreshTokensAsync(request.RefreshToken, cancellationToken);

        // 2. Map to Application DTO
        return new AuthTokensDto(
            coreResult.AccessToken ?? string.Empty,
            coreResult.RefreshToken ?? string.Empty,
            coreResult.ExpiresAt ?? DateTime.MinValue,
            coreResult.Role ?? string.Empty,
            coreResult.UserName ?? string.Empty);
    }
}