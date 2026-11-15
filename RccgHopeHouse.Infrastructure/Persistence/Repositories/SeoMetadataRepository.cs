using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="ISeoMetadataRepository"/>.
/// Handles persistence, content association, pagination and retrieval
/// of approved SEO metadata for the RCCG Hope House website.
/// </summary>
public class SeoMetadataRepository : ISeoMetadataRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the SEO metadata repository.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public SeoMetadataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<SeoMetadata?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.SeoMetadata
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<SeoMetadata?> GetByContentAsync(
        string contentType,
        Guid contentId,
        CancellationToken ct = default)
    {
        return await _context.SeoMetadata
            .FirstOrDefaultAsync(
                x => x.ContentType == contentType &&
                     x.ContentId == contentId,
                ct);
    }

    /// <inheritdoc />
    public async Task<SeoMetadata?> GetByPagePathAsync(
        string pagePath,
        CancellationToken ct = default)
    {
        return await _context.SeoMetadata
            .FirstOrDefaultAsync(
                x => x.PagePath == pagePath,
                ct);
    }

    /// <inheritdoc />
    public async Task<SeoMetadata?> GetActiveByContentAsync(
        string contentType,
        Guid contentId,
        CancellationToken ct = default)
    {
        return await _context.SeoMetadata
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.ContentType == contentType &&
                     x.ContentId == contentId &&
                     x.IsActive,
                ct);
    }

    /// <inheritdoc />
    public async Task<SeoMetadata?> GetActiveByPagePathAsync(
        string pagePath,
        CancellationToken ct = default)
    {
        return await _context.SeoMetadata
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PagePath == pagePath &&
                     x.IsActive,
                ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SeoMetadata>> GetPagedAsync(
        int skip,
        int take,
        string? contentType = null,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = _context.SeoMetadata
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            var type = contentType.Trim();

            query = query.Where(
                x => x.ContentType == type);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(
                x => x.ContentTitle.Contains(term) ||
                     x.SeoTitle.Contains(term));
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        string? contentType = null,
        string? search = null,
        CancellationToken ct = default)
    {
        var query = _context.SeoMetadata
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            var type = contentType.Trim();

            query = query.Where(
                x => x.ContentType == type);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(
                x => x.ContentTitle.Contains(term) ||
                     x.SeoTitle.Contains(term));
        }

        return await query.CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        SeoMetadata metadata,
        CancellationToken ct = default)
    {
        await _context.SeoMetadata.AddAsync(metadata, ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        SeoMetadata metadata,
        CancellationToken ct = default)
    {
        _context.SeoMetadata.Update(metadata);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Soft-deletes the metadata without physically removing its database row.
    /// The global BaseEntity query filter in ApplicationDbContext excludes
    /// soft-deleted records from subsequent normal queries.
    /// </remarks>
    public Task DeleteAsync(
        SeoMetadata metadata,
        CancellationToken ct = default)
    {
        metadata.SoftDelete();
        metadata.MarkAsUpdated();
        _context.SeoMetadata.Update(metadata);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
