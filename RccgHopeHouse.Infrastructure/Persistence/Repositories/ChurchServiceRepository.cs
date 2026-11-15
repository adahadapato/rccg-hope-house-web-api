using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of
/// <see cref="IChurchServiceRepository"/>.
/// </summary>
public sealed class ChurchServiceRepository
    : IChurchServiceRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ChurchServiceRepository"/> class.
    /// </summary>
    public ChurchServiceRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ChurchService?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.ChurchServices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                service => service.Id == id,
                ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchService>> GetAllAsync(
        ServiceCategory? category,
        DayOfWeek? dayOfWeek,
        bool? isActive,
        bool? isLocal,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query =
            _context.ChurchServices
                .AsNoTracking()
                .AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(
                service =>
                    service.Category == category.Value);
        }

        if (dayOfWeek.HasValue)
        {
            query = query.Where(
                service =>
                    service.DayOfWeek == dayOfWeek.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(
                service =>
                    service.IsActive == isActive.Value);
        }

        if (isLocal.HasValue)
        {
            query = query.Where(
                service =>
                    service.IsLocal == isLocal.Value);
        }

        return await query
            .OrderBy(
                service =>
                    service.DisplayOrder)
            .ThenBy(
                service =>
                    service.DayOfWeek)
            .ThenBy(
                service =>
                    service.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchService>>
        GetBroadcastEnabledAsync(
            CancellationToken ct = default) =>
        await _context.ChurchServices
            .AsNoTracking()
            .Where(
                service =>
                    service.IsActive &&
                    service.IsBroadcastEnabled)
            .OrderBy(
                service =>
                    service.DisplayOrder)
            .ThenBy(
                service =>
                    service.Name)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddAsync(
        ChurchService service,
        CancellationToken ct = default) =>
        await _context.ChurchServices
            .AddAsync(
                service,
                ct);

    /// <inheritdoc />
    public Task UpdateAsync(
        ChurchService service,
        CancellationToken ct = default)
    {
        _context.ChurchServices.Update(
            service);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        ChurchService service,
        CancellationToken ct = default)
    {
        _context.ChurchServices.Remove(
            service);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}