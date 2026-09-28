using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of
/// <see cref="IChurchEventRepository"/>.
///
/// Supports public Upcoming Events rendering
/// and administrative event management.
/// </summary>
public class ChurchEventRepository
    : IChurchEventRepository
{
    private readonly ApplicationDbContext _context;

    public ChurchEventRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ChurchEvent?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.ChurchEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(
                churchEvent =>
                    churchEvent.Id == id,
                ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchEvent>> GetAllAsync(
        ServiceCategory? category,
        bool? isActive,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context.ChurchEvents
            .AsNoTracking()
            .AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(
                churchEvent =>
                    churchEvent.Category ==
                    category.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(
                churchEvent =>
                    churchEvent.IsActive ==
                    isActive.Value);
        }

        return await query
            .OrderBy(
                churchEvent =>
                    churchEvent.StartDateTime)
            .ThenBy(
                churchEvent =>
                    churchEvent.DisplayOrder)
            .ThenBy(
                churchEvent =>
                    churchEvent.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchEvent>>
        GetUpcomingAsync(
            DateTime currentDateTime,
            int take,
            CancellationToken ct = default)
    {
        return await _context.ChurchEvents
            .AsNoTracking()
            .Where(
                churchEvent =>
                    churchEvent.IsActive &&
                    (
                        churchEvent.EndDateTime.HasValue
                            ? churchEvent.EndDateTime.Value >=
                              currentDateTime
                            : churchEvent.StartDateTime >=
                              currentDateTime
                    ))
            .OrderBy(
                churchEvent =>
                    churchEvent.StartDateTime)
            .ThenBy(
                churchEvent =>
                    churchEvent.DisplayOrder)
            .ThenBy(
                churchEvent =>
                    churchEvent.Title)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default)
    {
        await _context.ChurchEvents.AddAsync(
            churchEvent,
            ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default)
    {
        _context.ChurchEvents.Update(
            churchEvent);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default)
    {
        _context.ChurchEvents.Remove(
            churchEvent);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(
            ct);
    }
}