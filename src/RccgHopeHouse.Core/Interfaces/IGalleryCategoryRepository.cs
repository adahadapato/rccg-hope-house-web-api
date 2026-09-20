using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for gallery categories.
/// </summary>
public interface IGalleryCategoryRepository
{
    Task<GalleryCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<IReadOnlyList<GalleryCategory>> GetAllAsync(
        CancellationToken ct = default);

    Task<IReadOnlyList<GalleryCategory>> GetActiveAsync(
        CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default);

    Task AddAsync(
        GalleryCategory category,
        CancellationToken ct = default);

    Task UpdateAsync(
        GalleryCategory category,
        CancellationToken ct = default);

    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}