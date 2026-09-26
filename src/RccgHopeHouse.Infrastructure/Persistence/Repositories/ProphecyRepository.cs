using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core repository for individual prophecy persistence
/// and complete prophecy content retrieval.
/// </summary>
public class ProphecyRepository : IProphecyRepository
{
    private readonly ApplicationDbContext _context;

    public ProphecyRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Prophecy?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.Prophecies
            .FirstOrDefaultAsync(
                prophecy => prophecy.Id == id,
                ct);
    }

    public async Task<IReadOnlyList<Prophecy>> GetByCategoryAsync(
        Guid categoryId,
        bool activeOnly = false,
        CancellationToken ct = default)
    {
        var query = _context.Prophecies
            .AsNoTracking()
            .Where(
                prophecy =>
                    prophecy.CategoryId == categoryId);

        if (activeOnly)
        {
            query = query.Where(
                prophecy => prophecy.IsActive);
        }

        return await query
            .OrderBy(
                prophecy => prophecy.DisplayOrder)
            .ThenBy(
                prophecy => prophecy.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<ProphecyYear?> GetCompleteYearAsync(
        int year,
        bool publishedOnly = true,
        bool activeOnly = true,
        CancellationToken ct = default)
    {
        var query = _context.ProphecyYears
            .AsNoTracking()
            .Where(
                prophecyYear =>
                    prophecyYear.Year == year);

        if (publishedOnly)
        {
            query = query.Where(
                prophecyYear =>
                    prophecyYear.IsPublished);
        }

        query = query
            .Include(
                prophecyYear =>
                    prophecyYear.Categories
                        .Where(
                            category =>
                                !activeOnly ||
                                category.IsActive))
            .ThenInclude(
                category =>
                    category.Prophecies
                        .Where(
                            prophecy =>
                                !activeOnly ||
                                prophecy.IsActive));

        return await query
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ProphecyYear?>
        GetLatestCompletePublishedYearAsync(
            CancellationToken ct = default)
    {
        return await _context.ProphecyYears
            .AsNoTracking()
            .Where(
                year => year.IsPublished)
            .OrderByDescending(
                year => year.Year)
            .Include(
                year =>
                    year.Categories
                        .Where(
                            category =>
                                category.IsActive))
            .ThenInclude(
                category =>
                    category.Prophecies
                        .Where(
                            prophecy =>
                                prophecy.IsActive))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddAsync(
        Prophecy prophecy,
        CancellationToken ct = default)
    {
        await _context.Prophecies.AddAsync(
            prophecy,
            ct);
    }

    public Task UpdateAsync(
        Prophecy prophecy,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.Prophecies,
            prophecy);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        Prophecy prophecy,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.Prophecies,
            prophecy);

        _context.Prophecies.Remove(
            prophecy);

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