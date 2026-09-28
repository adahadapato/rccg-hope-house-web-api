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

    private static readonly TimeSpan TwoFactorChallengeLifetime =
        TimeSpan.FromMinutes(5);

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

        // Do not issue authentication tokens yet when
        // two-factor authentication is enabled.
        if (user.TwoFactorEnabled)
        {
            var challengeToken =
                await CreateTwoFactorChallengeAsync(
                    user.Id,
                    ct);

            return CreateTwoFactorRequiredResult(
                user,
                challengeToken);
        }

        return await CreateAuthenticatedResultAsync(
            user,
            ct);
    }

    /// <inheritdoc />
    public async Task<AuthResult> CompleteTwoFactorLoginAsync(
        string challengeToken,
        string verificationCode,
        bool useRecoveryCode,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(challengeToken) ||
            string.IsNullOrWhiteSpace(verificationCode))
        {
            throw new UnauthorizedAccessException(
                "The two-factor authentication challenge is invalid.");
        }

        var cacheKey =
            GetTwoFactorChallengeCacheKey(
                challengeToken);

        var userId =
            await _cache.GetStringAsync(
                cacheKey,
                ct);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "The two-factor authentication challenge is invalid or has expired.");
        }

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            await _cache.RemoveAsync(
                cacheKey,
                ct);

            throw new UnauthorizedAccessException(
                "The two-factor authentication challenge is invalid or has expired.");
        }

        if (!user.IsActive ||
            !user.EmailConfirmed ||
            !user.TwoFactorEnabled)
        {
            await _cache.RemoveAsync(
                cacheKey,
                ct);

            throw new UnauthorizedAccessException(
                "The two-factor authentication challenge is no longer valid.");
        }

        var code =
            verificationCode
                .Trim()
                .Replace(" ", string.Empty);

        bool isValid;

        if (useRecoveryCode)
        {
            var recoveryResult =
                await _userManager.RedeemTwoFactorRecoveryCodeAsync(
                    user,
                    code);

            isValid =
                recoveryResult.Succeeded;
        }
        else
        {
            code =
                code.Replace(
                    "-",
                    string.Empty);

            isValid =
                await _userManager.VerifyTwoFactorTokenAsync(
                    user,
                    _userManager.Options.Tokens
                        .AuthenticatorTokenProvider,
                    code);
        }

        if (!isValid)
        {
            throw new UnauthorizedAccessException(
                "The two-factor authentication code is invalid.");
        }

        // The challenge is single-use. Remove it before
        // issuing the real authentication tokens.
        await _cache.RemoveAsync(
            cacheKey,
            ct);

        return await CreateAuthenticatedResultAsync(
            user,
            ct);
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
            GetRefreshTokenCacheKey(
                refreshToken);

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

        // Do not allow an existing refresh token to bypass
        // a new email-verification requirement.
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
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var cacheKey =
            GetRefreshTokenCacheKey(
                refreshToken);

        await _cache.RemoveAsync(
            cacheKey,
            ct);

        return true;
    }

    /// <summary>
    /// Creates the final authenticated result and stores
    /// the refresh token.
    /// </summary>
    private async Task<AuthResult> CreateAuthenticatedResultAsync(
        ApplicationUser user,
        CancellationToken ct)
    {
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

    /// <summary>
    /// Creates a short-lived opaque two-factor challenge.
    /// Only its SHA-256 hash is used as the cache key.
    /// </summary>
    private async Task<string> CreateTwoFactorChallengeAsync(
        string userId,
        CancellationToken ct)
    {
        var randomBytes =
            RandomNumberGenerator.GetBytes(32);

        var challengeToken =
            Convert.ToBase64String(randomBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

        var cacheKey =
            GetTwoFactorChallengeCacheKey(
                challengeToken);

        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TwoFactorChallengeLifetime
            };

        await _cache.SetStringAsync(
            cacheKey,
            userId,
            options,
            ct);

        return challengeToken;
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
            GetRefreshTokenCacheKey(
                refreshToken);

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
            Encoding.UTF8.GetBytes(
                refreshToken);

        var hashBytes =
            SHA256.HashData(
                tokenBytes);

        var hash =
            Convert.ToHexString(
                hashBytes);

        return $"refresh_token:{hash}";
    }

    /// <summary>
    /// Creates the cache key for a two-factor challenge.
    /// The raw challenge token is never stored as the cache key.
    /// </summary>
    private static string GetTwoFactorChallengeCacheKey(
        string challengeToken)
    {
        var tokenBytes =
            Encoding.UTF8.GetBytes(
                challengeToken);

        var hashBytes =
            SHA256.HashData(
                tokenBytes);

        var hash =
            Convert.ToHexString(
                hashBytes);

        return $"two_factor_challenge:{hash}";
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
    /// Creates a result indicating that the password stage
    /// succeeded but two-factor verification is required.
    /// </summary>
    private static AuthResult CreateTwoFactorRequiredResult(
        ApplicationUser user,
        string challengeToken)
    {
        return new AuthResult(
            IsSuccess: false,
            AccessToken: null,
            RefreshToken: null,
            ExpiresAt: null,
            Role: null,
            ErrorMessage:
                "Two-factor authentication is required.",
            UserName: user.UserName,
            Name: GetFullName(user),
            Email: user.Email,
            RequiresTwoFactor: true,
            TwoFactorChallengeToken: challengeToken);
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
            .Select(name =>
                name.Trim()));
    }
}