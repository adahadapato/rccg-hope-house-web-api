using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core repository for prophecy year persistence.
/// </summary>
public class ProphecyYearRepository : IProphecyYearRepository
{
    private readonly ApplicationDbContext _context;

    public ProphecyYearRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProphecyYear?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .FirstOrDefaultAsync(
                year => year.Id == id,
                ct);
    }

    public async Task<ProphecyYear?> GetByYearAsync(
        int year,
        bool publishedOnly = false,
        CancellationToken ct = default)
    {
        var query = _context.ProphecyYears
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(
                item => item.IsPublished);
        }

        return await query.FirstOrDefaultAsync(
            item => item.Year == year,
            ct);
    }

    public async Task<IReadOnlyList<ProphecyYear>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .AsNoTracking()
            .OrderByDescending(year => year.Year)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProphecyYear>> GetPublishedAsync(
        CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .AsNoTracking()
            .Where(year => year.IsPublished)
            .OrderByDescending(year => year.Year)
            .ToListAsync(ct);
    }

    public async Task<ProphecyYear?> GetLatestPublishedAsync(
        CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .AsNoTracking()
            .Where(year => year.IsPublished)
            .OrderByDescending(year => year.Year)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsAsync(
        int year,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .AnyAsync(
                item =>
                    item.Year == year &&
                    (!excludeId.HasValue ||
                     item.Id != excludeId.Value),
                ct);
    }

    public async Task AddAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default)
    {
        await _context.ProphecyYears.AddAsync(
            prophecyYear,
            ct);
    }

    public Task UpdateAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.ProphecyYears,
            prophecyYear);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.ProphecyYears,
            prophecyYear);

        _context.ProphecyYears.Remove(
            prophecyYear);

        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    private void EnsureTracked<TEntity>(
        DbSet<TEntity> set,
        TEntity entity)
        where TEntity : class
    {
        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            set.Attach(entity);
            entry.State = EntityState.Modified;
        }
    }
}