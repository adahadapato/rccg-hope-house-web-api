using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IDevotionalRepository"/>.
/// Provides data access for public devotional reading
/// and admin devotional management.
/// </summary>
public class DevotionalRepository : IDevotionalRepository
{
    private readonly ApplicationDbContext _context;

    public DevotionalRepository(ApplicationDbContext context) =>
        _context = context;


    /// <inheritdoc />
    /// <remarks>
    /// Returns the most recent published devotional whose devotional
    /// date is today or earlier.
    ///
    /// If today's devotional is not published, the most recent
    /// previously published devotional is returned instead.
    /// Future devotionals are never returned by this query.
    /// </remarks>
    public async Task<Devotional?> GetLatestPublishedAsync(
        DateOnly today,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .AsNoTracking()
            .Where(d =>
                d.IsPublished &&
                d.DevotionalDate <= today)
            .OrderByDescending(d => d.DevotionalDate)
            .FirstOrDefaultAsync(ct);


    /// <inheritdoc />
    /// <remarks>
    /// Returns a published devotional for the exact requested date.
    /// The upToDate boundary prevents future devotionals from being
    /// exposed through the public API, even if they are published.
    /// </remarks>
    public async Task<Devotional?> GetPublishedByDateAsync(
        DateOnly devotionalDate,
        DateOnly upToDate,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d =>
                    d.IsPublished &&
                    d.DevotionalDate == devotionalDate &&
                    d.DevotionalDate <= upToDate,
                ct);


    /// <inheritdoc />
    /// <remarks>
    /// Returns published devotional history up to and including
    /// the supplied visibility boundary.
    ///
    /// This prevents future devotionals from appearing in the
    /// public devotional archive.
    /// </remarks>
    public async Task<IReadOnlyList<Devotional>> GetPublishedHistoryAsync(
        DateOnly upToDate,
        int skip = 0,
        int take = 30,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .AsNoTracking()
            .Where(d =>
                d.IsPublished &&
                d.DevotionalDate <= upToDate)
            .OrderByDescending(d => d.DevotionalDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);


    /// <inheritdoc />
    /// <remarks>
    /// Admin query intentionally includes every devotional:
    /// published, unpublished, past, current and future.
    /// </remarks>
    public async Task<IReadOnlyList<Devotional>> GetAllForAdminAsync(
        CancellationToken ct = default) =>
        await _context.Devotionals
            .AsNoTracking()
            .OrderByDescending(d => d.DevotionalDate)
            .ToListAsync(ct);


    /// <inheritdoc />
    public async Task<Devotional?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .FirstOrDefaultAsync(
                d => d.Id == id,
                ct);


    /// <inheritdoc />
    public async Task<Devotional?> GetByDateForAdminAsync(
        DateOnly devotionalDate,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .FirstOrDefaultAsync(
                d => d.DevotionalDate == devotionalDate,
                ct);


    /// <inheritdoc />
    /// <remarks>
    /// Used to prevent more than one devotional from being assigned
    /// to the same calendar date.
    ///
    /// During editing, excludeDevotionalId prevents the devotional
    /// being edited from conflicting with itself.
    /// </remarks>
    public async Task<bool> ExistsForDateAsync(
        DateOnly devotionalDate,
        Guid? excludeDevotionalId = null,
        CancellationToken ct = default)
    {
        var query = _context.Devotionals
            .AsNoTracking()
            .Where(d =>
                d.DevotionalDate == devotionalDate);

        if (excludeDevotionalId.HasValue)
        {
            query = query.Where(
                d => d.Id != excludeDevotionalId.Value);
        }

        return await query.AnyAsync(ct);
    }


    /// <inheritdoc />
    public async Task AddAsync(
        Devotional devotional,
        CancellationToken ct = default) =>
        await _context.Devotionals.AddAsync(
            devotional,
            ct);


    /// <inheritdoc />
    public Task UpdateAsync(
        Devotional devotional,
        CancellationToken ct = default)
    {
        _context.Devotionals.Update(devotional);

        return Task.CompletedTask;
    }


    /// <inheritdoc />
    public Task DeleteAsync(
        Devotional devotional,
        CancellationToken ct = default)
    {
        _context.Devotionals.Remove(devotional);

        return Task.CompletedTask;
    }


    /// <inheritdoc />
    public async Task<int> GetPublishedCountAsync(
        DateOnly upToDate,
        CancellationToken ct = default) =>
        await _context.Devotionals
            .AsNoTracking()
            .CountAsync(
                d =>
                    d.IsPublished &&
                    d.DevotionalDate <= upToDate,
                ct);


    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}