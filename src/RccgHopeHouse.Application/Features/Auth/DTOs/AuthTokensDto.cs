namespace RccgHopeHouse.Application.Features.Auth.Dtos;

/// <summary>
/// Response DTO containing JWT access/refresh tokens,
/// expiry information, and authenticated user details.
/// </summary>
public record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string Role,
    string UserName,
    string Name,
    string Email);