using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RccgHopeHouse.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Dedicated service for JWT access/refresh token generation and validation.
/// Separates token cryptography concerns from authentication logic in AuthService.
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly TimeSpan _accessTokenExpiry;
    private readonly TimeSpan _refreshTokenExpiry;

    /// <summary>
    /// Initializes the token service with configuration for cryptographic keys and expiry settings.
    /// </summary>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        var keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured"));
        _signingKey = new SymmetricSecurityKey(keyBytes);
        _accessTokenExpiry = TimeSpan.FromMinutes(double.Parse(configuration["Jwt:ExpiryMinutes"] ?? "60"));
        _refreshTokenExpiry = TimeSpan.FromDays(7);
    }

    /// <summary>
    /// Generates a new access/refresh token pair for an authenticated user.
    /// </summary>
    /// <param name="user">The authenticated ApplicationUser.</param>
    /// <param name="roles">The user's assigned roles for claim inclusion.</param>
    /// <returns>A tuple containing the access token, refresh token, and expiry timestamp.</returns>
    public (string AccessToken, string RefreshToken, DateTime ExpiresAt) GenerateTokens(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var accessToken = CreateJwtToken(claims, _accessTokenExpiry);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.Add(_accessTokenExpiry);

        return (accessToken, refreshToken, expiresAt);
    }

    /// <summary>
    /// Creates a signed JWT token with the specified claims and expiry.
    /// </summary>
    private string CreateJwtToken(IEnumerable<Claim> claims, TimeSpan expiry)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(expiry),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    /// <summary>
    /// Validates and extracts claims from an access token.
    /// Returns null if the token is invalid, expired, or tampered with.
    /// </summary>
    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = _signingKey,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extracts claims from an expired token for refresh flow validation.
    /// Does NOT validate expiry; caller must verify token was originally valid.
    /// </summary>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Allow expired for refresh
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = _signingKey
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }
}