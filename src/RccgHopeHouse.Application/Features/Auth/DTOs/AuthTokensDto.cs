namespace RccgHopeHouse.Application.Features.Auth.Dtos;

/// <summary>
/// Response DTO containing JWT access/refresh tokens and expiry.
/// </summary>
public record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Role,
    string UserName);