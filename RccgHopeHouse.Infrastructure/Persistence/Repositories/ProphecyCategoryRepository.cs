using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core repository for prophecy category persistence.
/// </summary>
public class ProphecyCategoryRepository
    : IProphecyCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public ProphecyCategoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProphecyCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.ProphecyCategories
            .Include(category => category.Prophecies)
            .FirstOrDefaultAsync(
                category => category.Id == id,
                ct);
    }

    public async Task<IReadOnlyList<ProphecyCategory>> GetByYearAsync(
        Guid prophecyYearId,
        bool activeOnly = false,
        CancellationToken ct = default)
    {
        var query = _context.ProphecyCategories
            .AsNoTracking()
            .Where(
                category =>
                    category.ProphecyYearId == prophecyYearId);

        if (activeOnly)
        {
            query = query.Where(
                category => category.IsActive);
        }

        return await query
            .Include(category => category.Prophecies)
            .OrderBy(
                category => category.DisplayOrder)
            .ThenBy(
                category => category.Name)
            .AsSplitQuery()
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid prophecyYearId,
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        var normalizedName = name.Trim();

        return await _context.ProphecyCategories
            .AnyAsync(
                category =>
                    category.ProphecyYearId == prophecyYearId &&
                    category.Name == normalizedName &&
                    (!excludeId.HasValue ||
                     category.Id != excludeId.Value),
                ct);
    }

    public async Task AddAsync(
        ProphecyCategory category,
        CancellationToken ct = default)
    {
        await _context.ProphecyCategories.AddAsync(
            category,
            ct);
    }

    public Task UpdateAsync(
        ProphecyCategory category,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.ProphecyCategories,
            category);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        ProphecyCategory category,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        EnsureTracked(
            _context.ProphecyCategories,
            category);

        _context.ProphecyCategories.Remove(
            category);

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