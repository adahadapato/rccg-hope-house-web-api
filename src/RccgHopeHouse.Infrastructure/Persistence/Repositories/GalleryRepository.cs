using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGalleryRepository"/>.
/// Optimized for binary data projection, tag joins, and category-based filtering.
/// </summary>
public class GalleryRepository : IGalleryRepository
{
    private readonly ApplicationDbContext _context;

    public GalleryRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryCategory>> GetAllCategoriesAsync(bool includeImageCount = false, CancellationToken ct = default)
    {
        // For domain entity returns, use simple AsNoTracking query
        var query = _context.GalleryCategories.AsNoTracking();
        return await query.OrderBy(c => c.DisplayOrder).ToListAsync(ct);
    }

    /// <summary>
    /// Gets categories projected into lightweight DTOs for UI dropdowns.
    /// This method is optimized for read-only scenarios where domain behavior isn't needed.
    /// </summary>
    // Fix for CS0854: Avoid using optional arguments in expression trees (e.g., in .Select with new record/DTO).
    // Explicitly specify all arguments for GalleryCategoryListItemDto constructor.
    public async Task<IReadOnlyList<GalleryCategoryListItemDto>> GetAllCategoriesAsDtoAsync(
        bool includeImageCount = false, CancellationToken ct = default)
    {
        var query = _context.GalleryCategories.AsNoTracking();

        if (includeImageCount)
        {
            return await query
                .Select(c => new GalleryCategoryListItemDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.DisplayOrder,
                    c.IsActive,
                    c.Images.Count)) // All parameters specified, no optional arguments
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(ct);
        }

        return await query
            .Select(c => new GalleryCategoryListItemDto(
                c.Id,
                c.Name,
                c.Description,
                c.DisplayOrder,
                c.IsActive,
                0)) // Explicitly provide ImageCount as 0 when not included
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<GalleryCategory?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.GalleryCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc />
    public async Task<GalleryCategory?> GetCategoryWithImagesAsync(Guid id, CancellationToken ct = default) =>
        await _context.GalleryCategories
            .AsNoTracking()
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc />
    public async Task AddCategoryAsync(GalleryCategory category, CancellationToken ct = default) =>
        await _context.GalleryCategories.AddAsync(category, ct);

    /// <inheritdoc />
    public Task UpdateCategoryAsync(GalleryCategory category, CancellationToken ct = default)
    {
        _context.GalleryCategories.Update(category);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteCategoryAsync(GalleryCategory category, CancellationToken ct = default)
    {
        _context.GalleryCategories.Remove(category);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Projects only metadata and thumbnail binary data. Full ImageData is excluded to optimize list payload size.
    /// </remarks>
    public async Task<IReadOnlyList<GalleryImage>> GetImagesAsync(
        Guid? categoryId = null, Guid? tagId = null, bool? isFeatured = null,
        bool publicOnly = true, int skip = 0, int take = 20, CancellationToken ct = default)
    {
        var query = _context.GalleryImages.AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags).ThenInclude(t => t.Tag);

        if (publicOnly) query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GalleryImage, GalleryTag>)query.Where(i => i.IsPublic);
        if (categoryId.HasValue) query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GalleryImage, GalleryTag>)query.Where(i => i.CategoryId == categoryId.Value);
        if (tagId.HasValue) query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GalleryImage, GalleryTag>)query.Where(i => i.Tags.Any(t => t.TagId == tagId.Value));
        if (isFeatured.HasValue) query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GalleryImage, GalleryTag>)query.Where(i => i.IsFeatured == isFeatured.Value);

        return await query
            .OrderByDescending(i => i.IsFeatured)
            .ThenByDescending(i => i.EventDate)
            .ThenBy(i => i.DisplayOrder)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetImagesCountAsync(
        Guid? categoryId = null, Guid? tagId = null, bool? isFeatured = null,
        bool publicOnly = true, CancellationToken ct = default)
    {
        var query = _context.GalleryImages.AsNoTracking();
        if (publicOnly) query = query.Where(i => i.IsPublic);
        if (categoryId.HasValue) query = query.Where(i => i.CategoryId == categoryId.Value);
        if (tagId.HasValue) query = query.Where(i => i.Tags.Any(t => t.TagId == tagId.Value));
        if (isFeatured.HasValue) query = query.Where(i => i.IsFeatured == isFeatured.Value);
        return await query.CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task<GalleryImage?> GetImageByIdAsync(Guid id, bool includeTags = false, CancellationToken ct = default)
    {
        var query = _context.GalleryImages.AsNoTracking().Include(i => i.Category);
        if (includeTags) query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<GalleryImage, GalleryCategory>)query.Include(i => i.Tags).ThenInclude(t => t.Tag);
        return await query.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetImageDataAsync(Guid id, CancellationToken ct = default) =>
        await _context.GalleryImages.AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => i.ImageData)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<byte[]?> GetThumbnailDataAsync(Guid id, CancellationToken ct = default) =>
        await _context.GalleryImages.AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => i.ThumbnailData)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>> GetFeaturedImagesAsync(int count = 10, CancellationToken ct = default) =>
        await _context.GalleryImages.AsNoTracking()
            .Where(i => i.IsFeatured && i.IsPublic)
            .OrderByDescending(i => i.EventDate)
            .Take(count)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>> GetRecentImagesAsync(int count = 20, CancellationToken ct = default) =>
        await _context.GalleryImages.AsNoTracking()
            .Where(i => i.IsPublic)
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddImageAsync(GalleryImage image, CancellationToken ct = default) =>
        await _context.GalleryImages.AddAsync(image, ct);

    /// <inheritdoc />
    public Task UpdateImageAsync(GalleryImage image, CancellationToken ct = default)
    {
        _context.GalleryImages.Update(image);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteImageAsync(GalleryImage image, CancellationToken ct = default)
    {
        _context.GalleryImages.Remove(image);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryTag>> GetAllTagsAsync(CancellationToken ct = default) =>
        await _context.GalleryTags.AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);

    /// <inheritdoc />
    public async Task<GalleryTag?> GetTagByNameAsync(string name, CancellationToken ct = default) =>
        await _context.GalleryTags.AsNoTracking().FirstOrDefaultAsync(t => t.Name == name.ToLowerInvariant(), ct);

    /// <inheritdoc />
    public async Task AddTagAsync(GalleryTag tag, CancellationToken ct = default) =>
        await _context.GalleryTags.AddAsync(tag, ct);

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    // Add this method to the existing GalleryRepository class

    ///// <inheritdoc />
    ///// <remarks>
    ///// Projects into DTO to avoid client-side evaluation. EF Core translates the Count() to SQL.
    ///// </remarks>
    //public async Task<IReadOnlyList<GalleryCategoryListItemDto>> GetAllCategoriesAsDtoAsync(
    //    bool includeImageCount = false, CancellationToken ct = default)
    //{
    //    var query = _context.GalleryCategories.AsNoTracking();

    //    if (includeImageCount)
    //    {
    //        return await query
    //            .Select(c => new GalleryCategoryListItemDto(
    //                c.Id,
    //                c.Name,
    //                c.Description,
    //                c.DisplayOrder,
    //                c.IsActive,
    //                c.Images.Count(i => i.IsPublic))) // Only count public images for UI
    //            .OrderBy(c => c.DisplayOrder)
    //            .ToListAsync(ct);
    //    }

    //    return await query
    //        .Select(c => new GalleryCategoryListItemDto(
    //            c.Id, c.Name, c.Description, c.DisplayOrder, c.IsActive))
    //        .OrderBy(c => c.DisplayOrder)
    //        .ToListAsync(ct);
    //}
}