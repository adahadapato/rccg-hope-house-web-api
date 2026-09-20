using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of gallery-category persistence.
/// </summary>
public class GalleryCategoryRepository : IGalleryCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public GalleryCategoryRepository(ApplicationDbContext context) =>
        _context = context;

    public async Task<GalleryCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.GalleryCategories
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<GalleryCategory>> GetAllAsync(
        CancellationToken ct = default) =>
        await _context.GalleryCategories
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<GalleryCategory>> GetActiveAsync(
        CancellationToken ct = default) =>
        await _context.GalleryCategories
            .Where(g => g.IsActive)
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default)
    {
        var normalizedName = name.Trim();

        return await _context.GalleryCategories
            .AnyAsync(
                g => g.Name == normalizedName &&
                     (!excludeId.HasValue || g.Id != excludeId.Value),
                ct);
    }

    public async Task AddAsync(
        GalleryCategory category,
        CancellationToken ct = default) =>
        await _context.GalleryCategories.AddAsync(category, ct);

    public Task UpdateAsync(
        GalleryCategory category,
        CancellationToken ct = default)
    {
        _context.GalleryCategories.Update(category);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}