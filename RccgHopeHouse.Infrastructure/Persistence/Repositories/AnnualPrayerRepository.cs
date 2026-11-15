using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of
/// <see cref="IAnnualPrayerRepository"/>.
/// </summary>
public class AnnualPrayerRepository
    : IAnnualPrayerRepository
{
    private readonly ApplicationDbContext _context;

    public AnnualPrayerRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<AnnualPrayer?> GetByYearAsync(
        int year,
        CancellationToken ct = default)
    {
        return await _context.AnnualPrayers
            .Include(p => p.PrayerPoints)
            .FirstOrDefaultAsync(
                p => p.Year == year,
                ct);
    }

    /// <inheritdoc />
    public async Task<AnnualPrayer?> GetActiveAsync(
        CancellationToken ct = default)
    {
        return await _context.AnnualPrayers
            .Include(p => p.PrayerPoints)
            .Where(p =>
                p.IsActive &&
                !p.IsDeleted)
            .OrderByDescending(p => p.Year)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<AnnualPrayer?> GetLatestAsync(
        CancellationToken ct = default)
    {
        return await _context.AnnualPrayers
            .Include(p => p.PrayerPoints)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.Year)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnnualPrayer>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _context.AnnualPrayers
            .Include(p => p.PrayerPoints)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.Year)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<AnnualPrayer?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.AnnualPrayers
            .Include(p => p.PrayerPoints)
            .FirstOrDefaultAsync(
                p =>
                    p.Id == id &&
                    !p.IsDeleted,
                ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default)
    {
        await _context.AnnualPrayers
            .AddAsync(
                annualPrayer,
                ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default)
    {
        _context.AnnualPrayers.Update(
            annualPrayer);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default)
    {
        _context.AnnualPrayers.Remove(
            annualPrayer);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemovePrayerPointsAsync(
        IEnumerable<AnnualPrayerPoint> prayerPoints,
        CancellationToken ct = default)
    {
        _context.AnnualPrayerPoints.RemoveRange(
            prayerPoints);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeactivateOtherActiveAsync(
        Guid annualPrayerId,
        CancellationToken ct = default)
    {
        var activePrayers =
            await _context.AnnualPrayers
                .Where(prayer =>
                    prayer.IsActive &&
                    prayer.Id != annualPrayerId &&
                    !prayer.IsDeleted)
                .ToListAsync(ct);

        foreach (var prayer in activePrayers)
        {
            prayer.Deactivate();
        }
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context
            .SaveChangesAsync(ct);
    }
}