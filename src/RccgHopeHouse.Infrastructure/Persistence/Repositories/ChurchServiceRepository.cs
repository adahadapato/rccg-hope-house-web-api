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
    public async Task<IReadOnlyList<ChurchService>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _context.ChurchServices.AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchService>> GetByCategoryAsync(ServiceCategory category, CancellationToken ct = default) =>
        await _context.ChurchServices.AsNoTracking()
            .Where(s => s.Category == category && s.IsActive)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChurchService>> GetByDayOfWeekAsync(DayOfWeek day, CancellationToken ct = default) =>
        await _context.ChurchServices.AsNoTracking()
            .Where(s => s.DayOfWeek == day && s.IsActive)
            .OrderBy(s => s.StartTime)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<ChurchService?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ChurchServices.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

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

    public Task<IEnumerable<ChurchService>> GetAllAsync(ServiceCategory? category, DayOfWeek? dayOfWeek, bool? isActive, int skip, int take, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}