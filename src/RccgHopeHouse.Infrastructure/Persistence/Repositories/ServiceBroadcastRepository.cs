using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IServiceBroadcastRepository"/>.
/// </summary>
public class ServiceBroadcastRepository : IServiceBroadcastRepository
{
    private readonly ApplicationDbContext _context;
    public ServiceBroadcastRepository(ApplicationDbContext context) => _context = context;

    public async Task<ServiceBroadcast?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ServiceBroadcasts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<ServiceBroadcast?> GetLatestByCategoryAsync(ServiceCategory category, CancellationToken ct = default) =>
        await _context.ServiceBroadcasts.AsNoTracking()
            .Where(b => b.Category == category)
            .OrderByDescending(b => b.ServiceMonth)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ServiceBroadcast>> GetLatestForCategoriesAsync(
        IEnumerable<ServiceCategory> categories, CancellationToken ct = default)
    {
        var categoryList = categories.ToList();

        // EF Core can't easily express "latest per group" in one query
        // portably, so this fetches per-category — fine at this scale
        // (3 categories, called rarely, cached implicitly by page load).
        var results = new List<ServiceBroadcast>();
        foreach (var category in categoryList)
        {
            var latest = await GetLatestByCategoryAsync(category, ct);
            if (latest != null) results.Add(latest);
        }
        return results;
    }

    public async Task<IReadOnlyList<ServiceBroadcast>> GetAllByCategoryAsync(
        ServiceCategory category, int skip, int take, CancellationToken ct = default) =>
        await _context.ServiceBroadcasts.AsNoTracking()
            .Where(b => b.Category == category)
            .OrderByDescending(b => b.ServiceMonth)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    public async Task AddAsync(ServiceBroadcast broadcast, CancellationToken ct = default) =>
        await _context.ServiceBroadcasts.AddAsync(broadcast, ct);

    public Task UpdateAsync(ServiceBroadcast broadcast, CancellationToken ct = default)
    {
        _context.ServiceBroadcasts.Update(broadcast);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ServiceBroadcast broadcast, CancellationToken ct = default)
    {
        _context.ServiceBroadcasts.Remove(broadcast);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}