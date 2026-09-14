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
    /// Exchanges a valid refresh token for a new access token pair.
    /// </summary>
    Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Validates a refresh token and issues a new access/refresh token pair.
    /// </summary>
    /// <param name="refreshToken">The valid refresh token to exchange.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>AuthTokensDto containing new tokens and user metadata.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when token is invalid, expired, or revoked.</exception>
    Task<AuthResult> RefreshTokensAsync(string refreshToken, CancellationToken ct = default);


    /// <summary>
    /// Invalidates a refresh token (e.g., on logout or password change).
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
}