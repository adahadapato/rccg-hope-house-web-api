using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IPrayerRequestRepository"/>.
/// Handles status-based filtering, pagination, and pastoral workflow queries.
/// </summary>
public class PrayerRequestRepository : IPrayerRequestRepository
{
    private readonly ApplicationDbContext _context;
    public PrayerRequestRepository(ApplicationDbContext context) => _context = context;


    // Add to existing PrayerRequestRepository

    /// <inheritdoc />
    public async Task<IReadOnlyList<PrayerRequest>> GetAllAsync(int skip, int take, CancellationToken ct = default) =>
        await _context.PrayerRequests.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        await _context.PrayerRequests.AsNoTracking().CountAsync(ct);

    /// <inheritdoc />
    public async Task<PrayerRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.PrayerRequests.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <inheritdoc />
    /// <remarks>
    /// Orders by CreatedAt descending. Supports status filtering for pastoral dashboard views.
    /// </remarks>
    public async Task<IReadOnlyList<PrayerRequest>> GetByStatusAsync(PrayerRequestStatus? status, int skip, int take, CancellationToken ct = default)
    {
        var query = _context.PrayerRequests.AsNoTracking();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        return await query.OrderByDescending(p => p.CreatedAt).Skip(skip).Take(take).ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetCountByStatusAsync(PrayerRequestStatus status, CancellationToken ct = default) =>
        await _context.PrayerRequests.AsNoTracking().CountAsync(p => p.Status == status, ct);

    /// <inheritdoc />
    public async Task AddAsync(PrayerRequest request, CancellationToken ct = default) =>
        await _context.PrayerRequests.AddAsync(request, ct);

    /// <inheritdoc />
    public Task UpdateAsync(PrayerRequest request, CancellationToken ct = default)
    {
        _context.PrayerRequests.Update(request);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public Task<IReadOnlyList<PrayerRequest>> GetByStatusAsync(PrayerRequestStatus status, CancellationToken ct = default, int skip = 0, int take = 0)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<PrayerRequest>> IPrayerRequestRepository.GetByStatusAsync(PrayerRequestStatus? status, int skip, int take, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountByStatusAsync(object pending, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}