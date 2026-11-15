using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

public interface IAnnouncementRepository
{
    Task<Announcement?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// All announcements for the admin News &amp; Updates page,
    /// newest start date first.
    /// </summary>
    Task<IReadOnlyList<Announcement>> GetAllAsync(
        bool? isPublished,
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Published announcements inside their display window.
    /// Used by the public floating banner.
    /// </summary>
    Task<IReadOnlyList<Announcement>> GetLiveAsync(
        DateTime utcNow,
        bool bannerOnly,
        int take,
        CancellationToken ct = default);

    Task AddAsync(
        Announcement announcement,
        CancellationToken ct = default);

    Task UpdateAsync(
        Announcement announcement,
        CancellationToken ct = default);

    Task DeleteAsync(
        Announcement announcement,
        CancellationToken ct = default);

    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}
