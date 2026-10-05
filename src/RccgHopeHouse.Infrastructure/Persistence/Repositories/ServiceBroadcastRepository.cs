using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of
/// <see cref="IServiceBroadcastRepository"/>.
/// </summary>
public sealed class ServiceBroadcastRepository
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
            .FirstOrDefaultAsync(
                broadcast => broadcast.Id == id,
                ct);

    /// <inheritdoc />
    public async Task<ServiceBroadcast?> GetByVideoIdAsync(
        string videoId,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            videoId,
            nameof(videoId));

        var normalizedVideoId =
            videoId.Trim();

        return await _context.ServiceBroadcasts
            .FirstOrDefaultAsync(
                broadcast =>
                    broadcast.VideoId == normalizedVideoId,
                ct);
    }

    /// <inheritdoc />
    public async Task<ServiceBroadcast?> GetByServiceAndMonthAsync(
        Guid churchServiceId,
        DateTime serviceMonth,
        CancellationToken ct = default)
    {
        if (churchServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A church service is required.",
                nameof(churchServiceId));
        }

        var monthStart =
            new DateTime(
                serviceMonth.Year,
                serviceMonth.Month,
                1);

        var nextMonth =
            monthStart.AddMonths(1);

        return await _context.ServiceBroadcasts
            .FirstOrDefaultAsync(
                broadcast =>
                    broadcast.ChurchServiceId == churchServiceId &&
                    broadcast.ServiceMonth >= monthStart &&
                    broadcast.ServiceMonth < nextMonth,
                ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceBroadcast>>
        GetByChurchServiceIdAsync(
            Guid churchServiceId,
            CancellationToken ct = default)
    {
        if (churchServiceId == Guid.Empty)
        {
            throw new ArgumentException(
                "A church service is required.",
                nameof(churchServiceId));
        }

        return await _context.ServiceBroadcasts
            .Where(
                broadcast =>
                    broadcast.ChurchServiceId == churchServiceId)
            .OrderByDescending(
                broadcast =>
                    broadcast.ServiceMonth)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<ServiceBroadcast?> GetLatestByCategoryAsync(
        ServiceCategory category,
        CancellationToken ct = default) =>
        await _context.ServiceBroadcasts
            .AsNoTracking()
            .Where(
                broadcast =>
                    broadcast.Category == category &&
                    broadcast.IsPublished &&
                    broadcast.ChurchService.IsActive &&
                    broadcast.ChurchService.IsBroadcastEnabled)
            .OrderByDescending(
                broadcast =>
                    broadcast.ServiceMonth)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceBroadcast>>
        GetLatestForBroadcastEnabledServicesAsync(
            CancellationToken ct = default)
    {
        var broadcasts =
            await _context.ServiceBroadcasts
                .AsNoTracking()
                .Where(
                    broadcast =>
                        broadcast.IsPublished &&
                        broadcast.ChurchService.IsActive &&
                        broadcast.ChurchService.IsBroadcastEnabled)
                .OrderByDescending(
                    broadcast =>
                        broadcast.ServiceMonth)
                .ToListAsync(ct);

        return broadcasts
            .GroupBy(
                broadcast =>
                    broadcast.ChurchServiceId)
            .Select(
                group =>
                    group.First())
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
                broadcast =>
                    broadcast.Category == category &&
                    broadcast.IsPublished &&
                    broadcast.ChurchService.IsActive &&
                    broadcast.ChurchService.IsBroadcastEnabled)
            .OrderByDescending(
                broadcast =>
                    broadcast.ServiceMonth)
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
                broadcast =>
                    broadcast.ServiceMonth)
            .ThenBy(
                broadcast =>
                    broadcast.Category)
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