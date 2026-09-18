using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IChurchServiceRepository"/>.
/// Optimized for public schedule rendering and admin management.
/// </summary>
public class ChurchServiceRepository : IChurchServiceRepository
{
    private readonly ApplicationDbContext _context;
    public ChurchServiceRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<ChurchService?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ChurchServices.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    /// <inheritdoc />
    /// <remarks>
    /// FIXED: previously an unimplemented stub throwing NotImplementedException
    /// — meaning GET /api/services was completely broken. Now implemented,
    /// with the new isLocal filter added alongside the existing ones.
    /// </remarks>
    public async Task<IReadOnlyList<ChurchService>> GetAllAsync(
        ServiceCategory? category,
        DayOfWeek? dayOfWeek,
        bool? isActive,
        bool? isLocal,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context.ChurchServices.AsNoTracking().AsQueryable();

        if (category.HasValue) query = query.Where(s => s.Category == category.Value);
        if (dayOfWeek.HasValue) query = query.Where(s => s.DayOfWeek == dayOfWeek.Value);
        if (isActive.HasValue) query = query.Where(s => s.IsActive == isActive.Value);
        if (isLocal.HasValue) query = query.Where(s => s.IsLocal == isLocal.Value);

        return await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(ChurchService service, CancellationToken ct = default) =>
        await _context.ChurchServices.AddAsync(service, ct);

    /// <inheritdoc />
    public Task UpdateAsync(ChurchService service, CancellationToken ct = default)
    {
        _context.ChurchServices.Update(service);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(ChurchService service, CancellationToken ct = default)
    {
        _context.ChurchServices.Remove(service);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}