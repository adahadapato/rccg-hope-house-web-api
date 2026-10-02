using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of
/// <see cref="IServiceBroadcastRepository"/>.
/// </summary>
/// <remarks>
/// Public latest-broadcast retrieval is based on the related
/// <see cref="ChurchService"/> configuration. A service must be
/// active and have broadcasting enabled.
///
/// Category-based methods are retained for existing functionality,
/// but service categories do not determine whether a service is
/// eligible to appear in the public broadcast feed.
/// </remarks>
public class ServiceBroadcastRepository
    : IServiceBroadcastRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ServiceBroadcastRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// Application database context.
    /// </param>
    public ServiceBroadcastRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ServiceBroadcast?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                b => b.Id == id,
                ct);

    /// <inheritdoc />
    public async Task<ServiceBroadcast?> GetLatestByCategoryAsync(
        ServiceCategory category,
        CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AsNoTracking()
            .Where(
                b => b.Category == category)
            .OrderByDescending(
                b => b.ServiceMonth)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceBroadcast>>
        GetLatestForBroadcastEnabledServicesAsync(
            CancellationToken ct = default)
    {
        /*
         * Retrieve broadcasts only for church services that:
         *
         * 1. are currently active; and
         * 2. explicitly have broadcasting enabled.
         *
         * This replaces the previous hard-coded category list.
         * The ChurchService relationship is authoritative for
         * deciding whether a broadcast belongs in the public feed.
         */
        var broadcasts =
            await _context.ServiceBroadcasts
                .AsNoTracking()
                .Where(
                    b =>
                        b.ChurchService.IsActive &&
                        b.ChurchService.IsBroadcastEnabled)
                .OrderByDescending(
                    b => b.ServiceMonth)
                .ToListAsync(ct);

        /*
         * A church service can have multiple historical broadcasts.
         *
         * Because the records above are already ordered from newest
         * to oldest, taking the first record from each ChurchServiceId
         * group gives us the latest broadcast for that particular
         * church service.
         *
         * Grouping by ChurchServiceId rather than Category is
         * important because multiple individual church services may
         * legitimately share the same ServiceCategory.
         */
        return broadcasts
            .GroupBy(
                b => b.ChurchServiceId)
            .Select(
                group => group.First())
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceBroadcast>>
        GetAllByCategoryAsync(
            ServiceCategory category,
            int skip,
            int take,
            CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AsNoTracking()
            .Where(
                b => b.Category == category)
            .OrderByDescending(
                b => b.ServiceMonth)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceBroadcast>>
        GetAllAsync(
            int skip,
            int take,
            CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AsNoTracking()
            .OrderByDescending(
                b => b.ServiceMonth)
            .ThenBy(
                b => b.Category)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AddAsync(
                broadcast,
                ct);

    /// <inheritdoc />
    public Task UpdateAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default)
    {
        _context.ServiceBroadcasts.Update(
            broadcast);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default)
    {
        _context.ServiceBroadcasts.Remove(
            broadcast);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}