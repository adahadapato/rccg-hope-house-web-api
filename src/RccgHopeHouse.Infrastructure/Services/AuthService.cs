using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;
using RccgHopeHouse.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IAuthService"/> using ASP.NET Core Identity and JWT token generation.
/// Handles credential validation, token issuance, and refresh rotation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenService _tokenService;
    private readonly IDistributedCache _cache;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
        JwtTokenService tokenService, IDistributedCache cache)
    {
        _userManager = userManager;
        _configuration = configuration;
        _tokenService = tokenService;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive || !await _userManager.CheckPasswordAsync(user, password))
            return new AuthResult(false, null, null, null, null, "Invalid email or password.", null);

        var roles = await _userManager.GetRolesAsync(user);
        var tokens = GenerateJwtTokens(user, roles);

        return new AuthResult(
                IsSuccess: true,
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken,
                ExpiresAt: tokens.ExpiresAt,
                Role: roles.FirstOrDefault() ?? "Member",
                ErrorMessage: "",
                UserName: user.UserName);
    }

    /// <inheritdoc />
    /// <summary>
    /// Validates a refresh token, checks revocation status, and issues a new token pair.
    /// </summary>
    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        // 1. Validate refresh token signature and extract claims
        var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
        if (principal?.Identity?.IsAuthenticated != true)
            throw new UnauthorizedAccessException("Invalid refresh token signature.");

        // 2. Extract user identifier from claims
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var userGuid))
            throw new UnauthorizedAccessException("Refresh token missing user identifier.");

        // 3. Check if token has been revoked (logout blacklisting)
        if (await IsTokenRevokedAsync(refreshToken, ct))
            throw new UnauthorizedAccessException("Refresh token has been revoked.");

        // 4. Load user and verify account status
        var user = await _userManager.FindByIdAsync(userGuid.ToString());
        if (user == null || !user.IsActive)
            throw new NotFoundException(nameof(ApplicationUser), userGuid);

        // 5. Get user roles for new token claims
        var roles = await _userManager.GetRolesAsync(user);

        // 6. Generate new token pair
        var (accessToken, newRefreshToken, expiresAt) = _tokenService.GenerateTokens(user, roles);

        return new AuthResult(
                IsSuccess: true,
                AccessToken: accessToken,
                RefreshToken: newRefreshToken,
                ExpiresAt: expiresAt,
                Role: roles.FirstOrDefault() ?? "Member",
                ErrorMessage: "",
                UserName: user.UserName);
    }

    /// <summary>
    /// Checks if a refresh token has been revoked by looking up its hash in the distributed cache.
    /// </summary>
    private async Task<bool> IsTokenRevokedAsync(string refreshToken, CancellationToken ct)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        var hash = Convert.ToBase64String(hashBytes);

        var cacheKey = $"revoked_token:{hash}";
        var cached = await _cache.GetStringAsync(cacheKey, ct);

        return !string.IsNullOrWhiteSpace(cached);
    }

    /// <inheritdoc />
    public Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        // ⚠️ STUB: does not actually write to _cache, so IsTokenRevokedAsync will
        // never find this token as revoked. Logout does not currently invalidate
        // refresh tokens. See open question below.
        return Task.FromResult(true);
    }

    /// <summary>
    /// Generates access and refresh JWT tokens with configured expiry and claims.
    /// </summary>
    private (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateJwtTokens(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expiry,
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return (accessToken, refreshToken, expiry);
    }
}