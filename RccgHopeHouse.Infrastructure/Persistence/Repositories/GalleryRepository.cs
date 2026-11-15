using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGalleryRepository"/>.
///
/// Handles gallery image metadata and tags stored in SQL Server.
/// Physical image files are handled separately through
/// <see cref="IGalleryImageStorage"/>.
///
/// Gallery category persistence is handled separately by
/// <see cref="IGalleryCategoryRepository"/>.
/// </summary>
public class GalleryRepository : IGalleryRepository
{
    private readonly ApplicationDbContext _context;

    public GalleryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

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
            .Include(image => image.Category)
            .Include(image => image.Tags)
                .ThenInclude(imageTag => imageTag.Tag)
            .AsQueryable();

        if (publicOnly)
        {
            query = query.Where(
                image => image.IsPublic);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                image =>
                    image.CategoryId ==
                    categoryId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(
                image =>
                    image.Tags.Any(
                        imageTag =>
                            imageTag.TagId ==
                            tagId.Value));
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(
                image =>
                    image.IsFeatured ==
                    isFeatured.Value);
        }

        return await query
            .OrderByDescending(
                image => image.IsFeatured)
            .ThenByDescending(
                image => image.EventDate)
            .ThenBy(
                image => image.DisplayOrder)
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
            query = query.Where(
                image => image.IsPublic);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(
                image =>
                    image.CategoryId ==
                    categoryId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(
                image =>
                    image.Tags.Any(
                        imageTag =>
                            imageTag.TagId ==
                            tagId.Value));
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(
                image =>
                    image.IsFeatured ==
                    isFeatured.Value);
        }

        return await query.CountAsync(ct);
    }

    /// <summary>
    /// Returns a tracked gallery image.
    ///
    /// Gallery images returned by this method are used by both
    /// query and command handlers. Keeping this entity tracked
    /// allows command handlers to modify the aggregate directly
    /// without reattaching the complete Category/Tag graph.
    ///
    /// Query handlers remain safe because they do not modify the
    /// returned entity unless their behaviour intentionally
    /// requires persistence, such as incrementing ViewCount.
    /// </summary>
    public async Task<GalleryImage?> GetImageByIdAsync(
        Guid id,
        bool includeTags = false,
        CancellationToken ct = default)
    {
        IQueryable<GalleryImage> query =
            _context.GalleryImages
                .Include(image => image.Category);

        if (includeTags)
        {
            query = query
                .Include(image => image.Tags)
                .ThenInclude(imageTag => imageTag.Tag);
        }

        return await query.FirstOrDefaultAsync(
            image => image.Id == id,
            ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>>
        GetFeaturedImagesAsync(
            int count = 10,
            CancellationToken ct = default)
    {
        return await _context.GalleryImages
            .AsNoTracking()
            .Include(image => image.Category)
            .Include(image => image.Tags)
                .ThenInclude(imageTag => imageTag.Tag)
            .Where(
                image =>
                    image.IsFeatured &&
                    image.IsPublic)
            .OrderByDescending(
                image => image.EventDate)
            .ThenBy(
                image => image.DisplayOrder)
            .Take(count)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImage>>
        GetRecentImagesAsync(
            int count = 20,
            CancellationToken ct = default)
    {
        return await _context.GalleryImages
            .AsNoTracking()
            .Include(image => image.Category)
            .Include(image => image.Tags)
                .ThenInclude(imageTag => imageTag.Tag)
            .Where(
                image => image.IsPublic)
            .OrderByDescending(
                image => image.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddImageAsync(
        GalleryImage image,
        CancellationToken ct = default)
    {
        await _context.GalleryImages.AddAsync(
            image,
            ct);
    }

    /// <summary>
    /// Gallery images loaded through GetImageByIdAsync are already
    /// tracked by this DbContext. No call to DbSet.Update is needed.
    ///
    /// Calling Update on the complete aggregate would attempt to
    /// attach its Category and Tag navigation objects again and can
    /// cause duplicate entity tracking conflicts.
    /// </summary>
    public Task UpdateImageAsync(
        GalleryImage image,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var entry =
            _context.Entry(image);

        if (entry.State ==
            EntityState.Detached)
        {
            /*
             * Defensive fallback.
             *
             * Normal command flow loads images through
             * GetImageByIdAsync, so this should rarely be needed.
             * Attach only the image itself rather than calling
             * DbSet.Update on the complete object graph.
             */
            _context.GalleryImages.Attach(
                image);

            entry =
                _context.Entry(image);

            entry.State =
                EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteImageAsync(
        GalleryImage image,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var entry =
            _context.Entry(image);

        if (entry.State ==
            EntityState.Detached)
        {
            _context.GalleryImages.Attach(
                image);
        }

        _context.GalleryImages.Remove(
            image);

        return Task.CompletedTask;
    }

    // ==================== Tags ====================

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryTag>>
        GetAllTagsAsync(
            CancellationToken ct = default)
    {
        return await _context.GalleryTags
            .AsNoTracking()
            .OrderBy(tag => tag.Name)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<GalleryTag?> GetTagByNameAsync(
        string name,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var normalizedName =
            name.Trim().ToLowerInvariant();

        /*
         * Do not use AsNoTracking here.
         *
         * During image editing, existing tags may already be
         * tracked because GetImageByIdAsync includes the image's
         * tag graph. EF Core's identity resolution will therefore
         * return the existing tracked GalleryTag instance instead
         * of creating a second instance with the same key.
         */
        return await _context.GalleryTags
            .FirstOrDefaultAsync(
                tag =>
                    tag.Name.ToLower() ==
                    normalizedName,
                ct);
    }

    /// <inheritdoc />
    public async Task AddTagAsync(
        GalleryTag tag,
        CancellationToken ct = default)
    {
        await _context.GalleryTags.AddAsync(
            tag,
            ct);
    }

    // ==================== Unit of Work ====================

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(
            ct);
    }
}