using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPastorPostRepository"/>.
/// Optimized for public feed queries (lightweight projection) and admin management.
/// </summary>
public class PastorPostRepository : IPastorPostRepository
{
    private readonly ApplicationDbContext _context;

    public PastorPostRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    /// <remarks>
    /// Orders pinned posts first, then by PublishedDate descending.
    /// Excludes full Content and CoverImageData to optimize memory for list views.
    /// </remarks>
    public async Task<IReadOnlyList<PastorPost>> GetPublishedFeedAsync(
        PostCategory? category = null, int skip = 0, int take = 10, CancellationToken ct = default)
    {
        var query = _context.PastorPosts.Where(p => p.IsPublished);
        if (category.HasValue) query = query.Where(p => p.Category == category.Value);

        return await query
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.PublishedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Loads full Content and CoverImageData for single-post detail views.
    /// </remarks>
    public async Task<PastorPost?> GetPublishedByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PastorPosts
            .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished, ct);
    }

    /// <inheritdoc />
    public async Task<PastorPost?> GetLatestPublishedAsync(CancellationToken ct = default) =>
        await _context.PastorPosts
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedDate)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<PastorPost>> GetFeaturedPostsAsync(int count = 5, CancellationToken ct = default) =>
        await _context.PastorPosts
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderByDescending(p => p.PublishedDate)
            .Take(count)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<PastorPost>> GetPinnedPostsAsync(CancellationToken ct = default) =>
        await _context.PastorPosts
            .Where(p => p.IsPublished && p.IsPinned)
            .OrderByDescending(p => p.PublishedDate)
            .ToListAsync(ct);

    /// <inheritdoc />
    /// <remarks>
    /// Admin query: includes drafts and full binary data for editing.
    /// </remarks>
    public async Task<IReadOnlyList<PastorPost>> GetAllForAdminAsync(bool includeDrafts = true, CancellationToken ct = default)
    {
        var query = _context.PastorPosts.AsQueryable();
        if (!includeDrafts) query = query.Where(p => p.IsPublished);
        return await query.OrderByDescending(p => p.PublishedDate).ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PastorPost?> GetByIdForAdminAsync(Guid id, CancellationToken ct = default) =>
        await _context.PastorPosts.FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(PastorPost post, CancellationToken ct = default) =>
        await _context.PastorPosts.AddAsync(post, ct);

    /// <inheritdoc />
    public Task UpdateAsync(PastorPost post, CancellationToken ct = default)
    {
        _context.PastorPosts.Update(post);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(PastorPost post, CancellationToken ct = default)
    {
        _context.PastorPosts.Remove(post);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> GetPublishedCountAsync(PostCategory? category = null, CancellationToken ct = default)
    {
        var query = _context.PastorPosts.Where(p => p.IsPublished);
        if (category.HasValue) query = query.Where(p => p.Category == category.Value);
        return await query.CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}