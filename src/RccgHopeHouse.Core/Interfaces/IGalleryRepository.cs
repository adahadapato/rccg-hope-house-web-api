using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for gallery image and tag persistence.
///
/// Physical gallery files are handled separately by
/// <see cref="IGalleryImageStorage"/>.
///
/// Gallery category operations are handled separately by
/// <see cref="IGalleryCategoryRepository"/>.
/// </summary>
public interface IGalleryRepository
{
    // ==================== Images ====================

    /// <summary>
    /// Retrieves paginated gallery images with optional filtering.
    /// </summary>
    Task<IReadOnlyList<GalleryImage>> GetImagesAsync(
        Guid? categoryId = null,
        Guid? tagId = null,
        bool? isFeatured = null,
        bool publicOnly = true,
        int skip = 0,
        int take = 20,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the number of gallery images matching the
    /// supplied filters.
    /// </summary>
    Task<int> GetImagesCountAsync(
        Guid? categoryId = null,
        Guid? tagId = null,
        bool? isFeatured = null,
        bool publicOnly = true,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single gallery image by ID.
    /// </summary>
    Task<GalleryImage?> GetImageByIdAsync(
        Guid id,
        bool includeTags = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets featured public gallery images.
    /// </summary>
    Task<IReadOnlyList<GalleryImage>> GetFeaturedImagesAsync(
        int count = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Gets recently uploaded public gallery images.
    /// </summary>
    Task<IReadOnlyList<GalleryImage>> GetRecentImagesAsync(
        int count = 20,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a gallery image.
    /// </summary>
    Task AddImageAsync(
        GalleryImage image,
        CancellationToken ct = default);

    /// <summary>
    /// Updates a gallery image.
    /// </summary>
    Task UpdateImageAsync(
        GalleryImage image,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a gallery image.
    /// </summary>
    Task DeleteImageAsync(
        GalleryImage image,
        CancellationToken ct = default);

    // ==================== Tags ====================

    /// <summary>
    /// Gets all gallery tags.
    /// </summary>
    Task<IReadOnlyList<GalleryTag>> GetAllTagsAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets a gallery tag by name.
    /// </summary>
    Task<GalleryTag?> GetTagByNameAsync(
        string name,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a gallery tag.
    /// </summary>
    Task AddTagAsync(
        GalleryTag tag,
        CancellationToken ct = default);

    // ==================== Unit of Work ====================

    /// <summary>
    /// Persists pending gallery changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}