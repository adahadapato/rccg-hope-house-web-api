using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Persistence for birthday wishes left on celebrant photos.
/// </summary>
public interface IGalleryImageCommentRepository
{
    /// <summary>Gets a wish for updating (tracked).</summary>
    Task<GalleryImageComment?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>Approved wishes for one photo, oldest first.</summary>
    Task<IReadOnlyList<GalleryImageComment>> GetApprovedForImageAsync(
        Guid galleryImageId,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Number of approved wishes per photo, for photos
    /// that have at least one.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, int>> GetApprovedCountsAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Wishes for the admin page, newest first, with the
    /// photo included so its title and thumbnail can be shown.
    /// </summary>
    Task<IReadOnlyList<GalleryImageComment>> GetAllAsync(
        BirthdayWishStatus? status,
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>Number of wishes with the given status.</summary>
    Task<int> CountAsync(
        BirthdayWishStatus status,
        CancellationToken ct = default);

    /// <summary>
    /// True when the photo exists, is public and is in a
    /// gallery category whose name contains "celebrant".
    /// </summary>
    Task<bool> IsPublicCelebrantImageAsync(
        Guid galleryImageId,
        CancellationToken ct = default);

    Task AddAsync(
        GalleryImageComment comment,
        CancellationToken ct = default);

    Task DeleteAsync(
        GalleryImageComment comment,
        CancellationToken ct = default);

    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}
