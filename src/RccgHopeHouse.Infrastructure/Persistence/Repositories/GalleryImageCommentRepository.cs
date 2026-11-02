using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGalleryImageCommentRepository"/>.
/// </summary>
public class GalleryImageCommentRepository
    : IGalleryImageCommentRepository
{
    /// <summary>
    /// Matches the website's celebrant highlight, which looks for
    /// gallery categories whose name contains "celebrant".
    /// </summary>
    private const string CelebrantCategoryKeyword = "celebrant";

    private readonly ApplicationDbContext _context;

    public GalleryImageCommentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<GalleryImageComment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.GalleryImageComments
            .Include(c => c.GalleryImage)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImageComment>> GetApprovedForImageAsync(
        Guid galleryImageId,
        int take,
        CancellationToken ct = default)
    {
        return await _context.GalleryImageComments
            .AsNoTracking()
            .Where(c =>
                c.GalleryImageId == galleryImageId &&
                c.Status == BirthdayWishStatus.Approved)
            .OrderBy(c => c.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<Guid, int>> GetApprovedCountsAsync(
        CancellationToken ct = default)
    {
        return await _context.GalleryImageComments
            .AsNoTracking()
            .Where(c => c.Status == BirthdayWishStatus.Approved)
            .GroupBy(c => c.GalleryImageId)
            .Select(g => new { ImageId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ImageId, x => x.Count, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GalleryImageComment>> GetAllAsync(
        BirthdayWishStatus? status,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context.GalleryImageComments
            .AsNoTracking()
            .Include(c => c.GalleryImage)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        BirthdayWishStatus status,
        CancellationToken ct = default)
    {
        return await _context.GalleryImageComments
            .CountAsync(c => c.Status == status, ct);
    }

    /// <inheritdoc />
    public async Task<bool> IsPublicCelebrantImageAsync(
        Guid galleryImageId,
        CancellationToken ct = default)
    {
        return await _context.GalleryImages
            .AsNoTracking()
            .AnyAsync(i =>
                i.Id == galleryImageId &&
                i.IsPublic &&
                i.Category.Name.Contains(CelebrantCategoryKeyword),
                ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        GalleryImageComment comment,
        CancellationToken ct = default)
    {
        await _context.GalleryImageComments.AddAsync(comment, ct);
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        GalleryImageComment comment,
        CancellationToken ct = default)
    {
        _context.GalleryImageComments.Remove(comment);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
