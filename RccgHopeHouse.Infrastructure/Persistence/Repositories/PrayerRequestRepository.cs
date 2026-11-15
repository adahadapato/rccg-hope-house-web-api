using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides the EF Core implementation of
/// <see cref="IPrayerRequestRepository"/>.
/// </summary>
/// <remarks>
/// Supports prayer request creation, retrieval, status filtering,
/// pagination, updates, deletion, and pastoral workflow queries.
/// </remarks>
public class PrayerRequestRepository : IPrayerRequestRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="PrayerRequestRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// The application's EF Core database context.
    /// </param>
    public PrayerRequestRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<PrayerRequest?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context
            .PrayerRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                prayerRequest =>
                    prayerRequest.Id == id,
                ct);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Results are ordered by creation date in descending order so
    /// that the newest prayer requests appear first.
    /// When <paramref name="status"/> is <c>null</c>, requests of
    /// all statuses are returned.
    /// </remarks>
    public async Task<IReadOnlyList<PrayerRequest>> GetByStatusAsync(
        PrayerRequestStatus? status,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context
            .PrayerRequests
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(
                prayerRequest =>
                    prayerRequest.Status ==
                    status.Value);
        }

        return await query
            .OrderByDescending(
                prayerRequest =>
                    prayerRequest.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetCountByStatusAsync(
        PrayerRequestStatus status,
        CancellationToken ct = default)
    {
        return await _context
            .PrayerRequests
            .AsNoTracking()
            .CountAsync(
                prayerRequest =>
                    prayerRequest.Status ==
                    status,
                ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        PrayerRequest request,
        CancellationToken ct = default)
    {
        await _context
            .PrayerRequests
            .AddAsync(
                request,
                ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        PrayerRequest request,
        CancellationToken ct = default)
    {
        _context
            .PrayerRequests
            .Update(request);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        PrayerRequest request,
        CancellationToken ct = default)
    {
        _context
            .PrayerRequests
            .Remove(request);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context
            .SaveChangesAsync(ct);
    }
}