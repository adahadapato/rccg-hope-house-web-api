using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAnnouncementRepository"/>.
/// </summary>
public class AnnouncementRepository
    : IAnnouncementRepository
{
    private readonly ApplicationDbContext _context;

    public AnnouncementRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Announcement?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.Announcements
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Announcement>> GetAllAsync(
        bool? isPublished,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query = _context.Announcements
            .AsNoTracking()
            .AsQueryable();

        if (isPublished.HasValue)
        {
            query = query.Where(a => a.IsPublished == isPublished.Value);
        }

        return await query
            .OrderByDescending(a => a.StartsAt)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Announcement>> GetLiveAsync(
        DateTime utcNow,
        bool bannerOnly,
        int take,
        CancellationToken ct = default)
    {
        var query = _context.Announcements
            .AsNoTracking()
            .Where(a =>
                a.IsPublished &&
                a.StartsAt <= utcNow &&
                (a.EndsAt == null || a.EndsAt >= utcNow));

        if (bannerOnly)
        {
            query = query.Where(a => a.ShowInBanner);
        }

        return await query
            .OrderBy(a => a.DisplayOrder)
            .ThenByDescending(a => a.StartsAt)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        Announcement announcement,
        CancellationToken ct = default)
    {
        await _context.Announcements.AddAsync(announcement, ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        Announcement announcement,
        CancellationToken ct = default)
    {
        _context.Announcements.Update(announcement);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        Announcement announcement,
        CancellationToken ct = default)
    {
        _context.Announcements.Remove(announcement);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
