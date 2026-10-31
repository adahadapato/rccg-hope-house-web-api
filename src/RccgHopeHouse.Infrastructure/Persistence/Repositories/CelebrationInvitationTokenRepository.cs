
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of the celebration invitation
/// token repository.
///
/// Invitation records are stored in SQL Server and tracked by the
/// application's existing ApplicationDbContext.
///
/// Token lookup uses the SHA-256 hash rather than the plaintext token.
/// </summary>
public sealed class CelebrationInvitationTokenRepository : ICelebrationInvitationTokenRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Creates a new invitation token repository.
    /// </summary>
    /// <param name="context">The application's database context.</param>
    public CelebrationInvitationTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<CelebrationInvitationToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        var normalizedHash = tokenHash.Trim().ToUpperInvariant();

        return await _context.Set<CelebrationInvitationToken>()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.TokenHash == normalizedHash, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CelebrationInvitationToken?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return null;

        return await _context.Set<CelebrationInvitationToken>()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CelebrationInvitationToken>> GetByCelebrationAsync(
        Guid celebrationId,
        CancellationToken cancellationToken = default)
    {
        if (celebrationId == Guid.Empty)
            return Array.Empty<CelebrationInvitationToken>();

        return await _context.Set<CelebrationInvitationToken>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CelebrationId == celebrationId)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CelebrationInvitationToken>> GetActiveByCelebrationAsync(
        Guid celebrationId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (celebrationId == Guid.Empty)
            return Array.Empty<CelebrationInvitationToken>();

        if (nowUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("The current date and time must be UTC.", nameof(nowUtc));

        return await _context.Set<CelebrationInvitationToken>()
            .Where(x =>
                !x.IsDeleted &&
                x.CelebrationId == celebrationId &&
                x.IssuedAt <= nowUtc &&
                x.ExpiresAt > nowUtc &&
                x.UsedAt == null &&
                x.RevokedAt == null)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        CelebrationInvitationToken invitation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invitation);

        await _context.Set<CelebrationInvitationToken>()
            .AddAsync(invitation, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        CelebrationInvitationToken invitation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(invitation);
        cancellationToken.ThrowIfCancellationRequested();

        _context.Set<CelebrationInvitationToken>().Update(invitation);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
