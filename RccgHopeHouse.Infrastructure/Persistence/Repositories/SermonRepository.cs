using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ISermonRepository"/>.
/// Handles querying, pagination, and persistence of sermon records.
/// </summary>
public class SermonRepository : ISermonRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance with the provided DbContext.
    /// </summary>
    public SermonRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<Sermon?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Sermons.FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc />
    /// <remarks>
    /// Uses server-side pagination and search filtering. Only returns published sermons.
    /// Orders by ServiceDate descending for chronological feeds.
    /// </remarks>
    public async Task<IReadOnlyList<Sermon>> GetPagedAsync(int skip, int take, string? search, CancellationToken ct = default)
    {
        var query = _context.Sermons.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Title.Contains(search) || s.Speaker.Contains(search));

        return await query
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.ServiceDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Sermons.Where(s => s.IsPublished);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Title.Contains(search) || s.Speaker.Contains(search));
        return await query.CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(Sermon sermon, CancellationToken ct = default) =>
        await _context.Sermons.AddAsync(sermon, ct);

    /// <inheritdoc />
    public Task UpdateAsync(Sermon sermon, CancellationToken ct = default)
    {
        _context.Sermons.Update(sermon);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Sermon sermon, CancellationToken ct = default)
    {
        _context.Sermons.Remove(sermon);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Sermon>> GetAllPublishedAsync(int skip, int take, CancellationToken ct = default)
    {
        return await _context.Sermons
            .AsNoTracking()
            .Where(s => s.IsPublished)
            .OrderByDescending(s => s.ServiceDate) // Consistent ordering for pagination
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> CountPublishedAsync(CancellationToken ct = default)
    {
        return await _context.Sermons
            .AsNoTracking()
            .CountAsync(s => s.IsPublished, ct);
    }

    public async Task<IReadOnlyList<Sermon>> GetPagedAsync(
    int skip,
    int take,
    string? search,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    CancellationToken ct = default)
    {
        var query = _context.Sermons.AsNoTracking().Where(s => s.IsPublished);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Title.Contains(search) || s.Speaker.Contains(search));

        if (fromDate.HasValue)
            query = query.Where(s => s.ServiceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.ServiceDate <= toDate.Value);

        return await query
            .OrderByDescending(s => s.ServiceDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
}