using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

public class FirstTimerRepository : IFirstTimerRepository
{
    private readonly ApplicationDbContext _context;

    public FirstTimerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FirstTimer?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context
            .FirstTimers
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<FirstTimer?> GetByNormalizedPhoneAsync(
        string normalizedPhoneNumber,
        CancellationToken ct = default)
    {
        return await _context
            .FirstTimers
            .OrderByDescending(f => f.CreatedAt)
            .FirstOrDefaultAsync(
                f => f.NormalizedPhoneNumber == normalizedPhoneNumber,
                ct);
    }

    public async Task<IReadOnlyList<FirstTimer>> GetListAsync(
        FirstTimerFollowUpStatus? status,
        string? search,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context
            .FirstTimers
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(f => f.FollowUpStatus == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(f =>
                f.FirstName.Contains(term) ||
                f.LastName.Contains(term) ||
                f.PhoneNumber.Contains(term) ||
                (f.Email != null && f.Email.Contains(term)));
        }

        return await query
            .OrderByDescending(f => f.LastVisitDate)
            .ThenByDescending(f => f.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByStatusAsync(
        FirstTimerFollowUpStatus status,
        CancellationToken ct = default)
    {
        return await _context
            .FirstTimers
            .AsNoTracking()
            .CountAsync(f => f.FollowUpStatus == status, ct);
    }

    public async Task<int> GetPromotedCountAsync(
        CancellationToken ct = default)
    {
        return await _context
            .FirstTimers
            .AsNoTracking()
            .CountAsync(f => f.MemberId != null, ct);
    }

    public async Task AddAsync(
        FirstTimer firstTimer,
        CancellationToken ct = default)
    {
        await _context.FirstTimers.AddAsync(firstTimer, ct);
    }

    public Task DeleteAsync(
        FirstTimer firstTimer,
        CancellationToken ct = default)
    {
        _context.FirstTimers.Remove(firstTimer);

        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
