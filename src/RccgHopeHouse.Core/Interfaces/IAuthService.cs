using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Authentication service contract for login, two-factor authentication,
/// token refresh, and revocation.
/// Implemented in Infrastructure using ASP.NET Core Identity + JWT.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates the user's email address and password.
    /// If two-factor authentication is enabled, no access or refresh
    /// token is issued until the second factor has been verified.
    /// </summary>
    Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default);

    /// <summary>
    /// Completes a pending two-factor authentication challenge using
    /// either an authenticator code or a recovery code.
    /// </summary>
    Task<AuthResult> CompleteTwoFactorLoginAsync(
        string challengeToken,
        string verificationCode,
        bool useRecoveryCode,
        CancellationToken ct = default);

    /// <summary>
    /// Validates a refresh token, checks revocation status, and issues
    /// a new access/refresh token pair.
    /// </summary>
    /// <param name="refreshToken">
    /// The valid refresh token to exchange.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// An <see cref="AuthResult"/> containing new tokens
    /// and user metadata.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the token is invalid, expired, or revoked.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the user associated with the token
    /// no longer exists.
    /// </exception>
    Task<AuthResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default);

    /// <summary>
    /// Invalidates a refresh token, for example on logout.
    /// </summary>
    Task<bool> RevokeTokenAsync(
        string refreshToken,
        CancellationToken ct = default);
}