using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IThanksgivingRepository"/>.
/// Optimized for YouTube sync deduplication and monthly public feeds.
/// </summary>
public class ThanksgivingRepository : IThanksgivingRepository
{
    private readonly ApplicationDbContext _context;
    public ThanksgivingRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<ThanksgivingService?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ThanksgivingServices.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    /// <inheritdoc />
    public async Task<ThanksgivingService?> GetByVideoUrlAsync(string videoUrl, CancellationToken ct = default) =>
        await _context.ThanksgivingServices.AsNoTracking()
            .FirstOrDefaultAsync(t => t.VideoUrl.Contains(videoUrl), ct);

    /// <inheritdoc />
    public async Task<ThanksgivingService?> GetByMonthAsync(DateTime month, CancellationToken ct = default)
    {
        var target = new DateTime(month.Year, month.Month, 1);
        return await _context.ThanksgivingServices.AsNoTracking()
            .FirstOrDefaultAsync(t => t.ServiceMonth.Year == target.Year && t.ServiceMonth.Month == target.Month, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ThanksgivingService>> GetAsync(DateTime fromMonth, DateTime toMonth, CancellationToken ct = default) =>
        await _context.ThanksgivingServices.AsNoTracking()
            .Where(t => t.ServiceMonth >= fromMonth && t.ServiceMonth <= toMonth)
            .OrderByDescending(t => t.ServiceMonth)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddAsync(ThanksgivingService service, CancellationToken ct = default) =>
        await _context.ThanksgivingServices.AddAsync(service, ct);

    /// <inheritdoc />
    public Task UpdateAsync(ThanksgivingService service, CancellationToken ct = default)
    {
        _context.ThanksgivingServices.Update(service);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    //public async Task<YouTubeVideoInfo> GetByVideoUrlAsync(string videoUrl, CancellationToken ct)
    //{
    //    throw new NotImplementedException();
    //}
}