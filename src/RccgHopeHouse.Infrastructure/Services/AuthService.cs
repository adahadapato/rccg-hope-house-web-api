using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;
using RccgHopeHouse.Infrastructure.Identity;
using System.Security.Cryptography;
using System.Text;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IAuthService"/> using ASP.NET Core Identity
/// and JWT access tokens with opaque refresh tokens.
/// </summary>
public class AuthService : IAuthService
{
    private static readonly TimeSpan RefreshTokenLifetime =
        TimeSpan.FromDays(7);

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtTokenService _tokenService;
    private readonly IDistributedCache _cache;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        JwtTokenService tokenService,
        IDistributedCache cache)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await _userManager.FindByEmailAsync(email);

        // Keep invalid credentials and inactive accounts
        // behind the same generic authentication response.
        if (user is null ||
            !user.IsActive ||
            !await _userManager.CheckPasswordAsync(
                user,
                password))
        {
            return CreateFailedLoginResult();
        }

        // Credentials are valid, but the account must verify
        // its email address before authentication can complete.
        if (!user.EmailConfirmed)
        {
            return CreateEmailNotConfirmedResult(user);
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var tokens =
            _tokenService.GenerateTokens(
                user,
                roles);

        await StoreRefreshTokenAsync(
            tokens.RefreshToken,
            user.Id,
            ct);

        return CreateSuccessfulAuthResult(
            user,
            roles,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresAt);
    }

    /// <inheritdoc />
    public async Task<AuthResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh token is required.");
        }

        var cacheKey =
            GetRefreshTokenCacheKey(refreshToken);

        var userId =
            await _cache.GetStringAsync(
                cacheKey,
                ct);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "The refresh token is invalid, expired, or revoked.");
        }

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            await _cache.RemoveAsync(
                cacheKey,
                ct);

            throw new NotFoundException(
                nameof(ApplicationUser),
                userId);
        }

        if (!user.IsActive)
        {
            await _cache.RemoveAsync(
                cacheKey,
                ct);

            throw new UnauthorizedAccessException(
                "This account is inactive.");
        }

        // If the email address has changed since the refresh
        // token was issued, UpdateUserAsync resets
        // EmailConfirmed to false. Do not allow that existing
        // refresh token to bypass the new verification requirement.
        if (!user.EmailConfirmed)
        {
            await _cache.RemoveAsync(
                cacheKey,
                ct);

            throw new UnauthorizedAccessException(
                "Please verify your email address before signing in.");
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var tokens =
            _tokenService.GenerateTokens(
                user,
                roles);

        // Rotate the refresh token.
        // The old token becomes unusable before the new token is stored.
        await _cache.RemoveAsync(
            cacheKey,
            ct);

        await StoreRefreshTokenAsync(
            tokens.RefreshToken,
            user.Id,
            ct);

        return CreateSuccessfulAuthResult(
            user,
            roles,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresAt);
    }

    /// <inheritdoc />
    public async Task<bool> RevokeTokenAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var cacheKey =
            GetRefreshTokenCacheKey(refreshToken);

        await _cache.RemoveAsync(
            cacheKey,
            ct);

        return true;
    }

    /// <summary>
    /// Stores the association between a hashed refresh token
    /// and the user who owns it.
    /// </summary>
    private async Task StoreRefreshTokenAsync(
        string refreshToken,
        string userId,
        CancellationToken ct)
    {
        var cacheKey =
            GetRefreshTokenCacheKey(refreshToken);

        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    RefreshTokenLifetime
            };

        await _cache.SetStringAsync(
            cacheKey,
            userId,
            options,
            ct);
    }

    /// <summary>
    /// Creates the cache key for a refresh token.
    /// The raw refresh token is never stored as the cache key.
    /// </summary>
    private static string GetRefreshTokenCacheKey(
        string refreshToken)
    {
        var tokenBytes =
            Encoding.UTF8.GetBytes(refreshToken);

        var hashBytes =
            SHA256.HashData(tokenBytes);

        var hash =
            Convert.ToHexString(hashBytes);

        return $"refresh_token:{hash}";
    }

    /// <summary>
    /// Creates a standard failed authentication result.
    /// </summary>
    private static AuthResult CreateFailedLoginResult()
    {
        return new AuthResult(
            IsSuccess: false,
            AccessToken: null,
            RefreshToken: null,
            ExpiresAt: null,
            Role: null,
            ErrorMessage: "Invalid email or password.",
            UserName: null,
            Name: null,
            Email: null);
    }

    /// <summary>
    /// Creates an authentication result for a valid account
    /// whose email address has not yet been confirmed.
    /// </summary>
    private static AuthResult CreateEmailNotConfirmedResult(
        ApplicationUser user)
    {
        return new AuthResult(
            IsSuccess: false,
            AccessToken: null,
            RefreshToken: null,
            ExpiresAt: null,
            Role: null,
            ErrorMessage:
                "Please verify your email address before signing in.",
            UserName: user.UserName,
            Name: GetFullName(user),
            Email: user.Email);
    }

    /// <summary>
    /// Creates a successful authentication result.
    /// </summary>
    private static AuthResult CreateSuccessfulAuthResult(
        ApplicationUser user,
        IList<string> roles,
        string accessToken,
        string refreshToken,
        DateTime expiresAt)
    {
        return new AuthResult(
            IsSuccess: true,
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: expiresAt,
            Role: roles.FirstOrDefault() ?? "Member",
            ErrorMessage: string.Empty,
            UserName: user.UserName,
            Name: GetFullName(user),
            Email: user.Email);
    }

    /// <summary>
    /// Builds the user's display name from their
    /// first and last names.
    /// </summary>
    private static string GetFullName(
        ApplicationUser user)
    {
        return string.Join(
            " ",
            new[]
            {
                user.FirstName,
                user.LastName
            }
            .Where(name =>
                !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim()));
    }
}