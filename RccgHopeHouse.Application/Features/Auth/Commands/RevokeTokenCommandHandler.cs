using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Security.Cryptography;

namespace RccgHopeHouse.Application.Features.Auth.Commands;

/// <summary>
/// Handler for revoking refresh tokens during logout.
/// Implements token blacklisting via distributed cache to prevent reuse.
/// </summary>
public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Unit>
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _revocationWindow;

    /// <summary>
    /// Initializes the handler with cache and token expiry configuration.
    /// </summary>
    public RevokeTokenCommandHandler(IDistributedCache cache)
    {
        _cache = cache;
        // Keep revoked tokens in cache for the duration of their original expiry
        _revocationWindow = TimeSpan.FromDays(7);
    }

    /// <summary>
    /// Stores the token hash in cache with an expiry matching the token's lifetime.
    /// Subsequent refresh attempts check this cache and reject blacklisted tokens.
    /// </summary>
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        // Hash the token to avoid storing raw values in cache
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
        var hash = Convert.ToBase64String(hashBytes);

        var cacheKey = $"revoked_token:{hash}";

        // Store with sliding expiry matching token lifetime
        await _cache.SetStringAsync(
            cacheKey,
            "revoked",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _revocationWindow
            },
            cancellationToken);

        return Unit.Value;
    }
}