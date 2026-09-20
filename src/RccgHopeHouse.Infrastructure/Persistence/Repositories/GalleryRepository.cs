using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGalleryRepository"/>.
/// Handles gallery images and tags.
///
/// Gallery category persistence is handled separately by
/// <see cref="IGalleryCategoryRepository"/>.
/// </summary>
public class GalleryRepository : IGalleryRepository
{
    private readonly ApplicationDbContext _context;

    public GalleryRepository(ApplicationDbContext context) =>
        _context = context;

    // ==================== Images ====================

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>> GetImagesAsync(
        Guid? categoryId = null,
        Guid? tagId = null,
        bool? isFeatured = null,
        bool publicOnly = true,
        int skip = 0,
        int take = 20,
        CancellationToken ct = default)
    {
        var query = _context.GalleryImages
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Tags)
                .ThenInclude(t => t.Tag)
            .AsQueryable();

        if (publicOnly)
        {
            query = query.Where(i => i.IsPublic);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                i => i.CategoryId == categoryId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(
                i => i.Tags.Any(t => t.TagId == tagId.Value));
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(
                i => i.IsFeatured == isFeatured.Value);
        }

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
        Guid? categoryId = null,
        Guid? tagId = null,
        bool? isFeatured = null,
        bool publicOnly = true,
        CancellationToken ct = default)
    {
        var query = _context.GalleryImages
            .AsNoTracking()
            .AsQueryable();

        if (publicOnly)
        {
            query = query.Where(i => i.IsPublic);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                i => i.CategoryId == categoryId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(
                i => i.Tags.Any(t => t.TagId == tagId.Value));
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(
                i => i.IsFeatured == isFeatured.Value);
        }

        return await query.CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task<GalleryImage?> GetImageByIdAsync(
        Guid id,
        bool includeTags = false,
        CancellationToken ct = default)
    {
        IQueryable<GalleryImage> query = _context.GalleryImages
            .AsNoTracking()
            .Include(i => i.Category);

        if (includeTags)
        {
            query = query
                .Include(i => i.Tags)
                .ThenInclude(t => t.Tag);
        }

        return await query
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<byte[]?> GetImageDataAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.GalleryImages
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => i.ImageData)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<byte[]?> GetThumbnailDataAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.GalleryImages
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => i.ThumbnailData)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>> GetFeaturedImagesAsync(
        int count = 10,
        CancellationToken ct = default) =>
        await _context.GalleryImages
            .AsNoTracking()
            .Where(i => i.IsFeatured && i.IsPublic)
            .OrderByDescending(i => i.EventDate)
            .Take(count)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>> GetRecentImagesAsync(
        int count = 20,
        CancellationToken ct = default) =>
        await _context.GalleryImages
            .AsNoTracking()
            .Where(i => i.IsPublic)
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddImageAsync(
        GalleryImage image,
        CancellationToken ct = default) =>
        await _context.GalleryImages.AddAsync(image, ct);

    /// <inheritdoc />
    public Task UpdateImageAsync(
        GalleryImage image,
        CancellationToken ct = default)
    {
        _context.GalleryImages.Update(image);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteImageAsync(
        GalleryImage image,
        CancellationToken ct = default)
    {
        _context.GalleryImages.Remove(image);
        return Task.CompletedTask;
    }

    // ==================== Tags ====================

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryTag>> GetAllTagsAsync(
        CancellationToken ct = default) =>
        await _context.GalleryTags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<GalleryTag?> GetTagByNameAsync(
        string name,
        CancellationToken ct = default) =>
        await _context.GalleryTags
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.Name == name.ToLowerInvariant(),
                ct);

    /// <inheritdoc />
    public async Task AddTagAsync(
        GalleryTag tag,
        CancellationToken ct = default) =>
        await _context.GalleryTags.AddAsync(tag, ct);

    // ==================== Unit of Work ====================

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}