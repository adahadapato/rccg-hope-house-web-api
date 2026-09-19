using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGivingTypeRepository"/>.
/// Provides persistence operations for giving types.
/// </summary>
public class GivingTypeRepository : IGivingTypeRepository
{
    private readonly ApplicationDbContext _context;

    public GivingTypeRepository(ApplicationDbContext context) =>
        _context = context;

    /// <inheritdoc />
    public async Task<GivingType?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.GivingTypes
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GivingType>> GetAllAsync(
        CancellationToken ct = default) =>
        await _context.GivingTypes
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Name)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GivingType>> GetActiveAsync(
        CancellationToken ct = default) =>
        await _context.GivingTypes
            .Where(g => g.IsActive)
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Name)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        var normalizedName = name.Trim();

        return await _context.GivingTypes
            .AnyAsync(
                g => g.Name == normalizedName
                     && (!excludeId.HasValue || g.Id != excludeId.Value),
                ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        GivingType givingType,
        CancellationToken ct = default) =>
        await _context.GivingTypes.AddAsync(givingType, ct);

    /// <inheritdoc />
    public Task UpdateAsync(
        GivingType givingType,
        CancellationToken ct = default)
    {
        _context.GivingTypes.Update(givingType);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}