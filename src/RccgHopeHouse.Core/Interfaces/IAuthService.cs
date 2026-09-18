using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Authentication service contract for login, token refresh, and revocation.
/// Implemented in Infrastructure using ASP.NET Core Identity + JWT.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and returns JWT access/refresh tokens.
    /// </summary>
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Validates a refresh token, checks revocation status, and issues a new
    /// access/refresh token pair.
    /// </summary>
    /// <param name="refreshToken">The valid refresh token to exchange.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>An <see cref="AuthResult"/> containing new tokens and user metadata.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the token is invalid, expired, or revoked.</exception>
    /// <exception cref="NotFoundException">Thrown when the user associated with the token no longer exists or is inactive.</exception>
    Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Invalidates a refresh token (e.g., on logout or password change).
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
}