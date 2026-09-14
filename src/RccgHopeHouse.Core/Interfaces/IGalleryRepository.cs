using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for gallery operations.
/// Defines data access contracts for images, categories, and tags.
/// Implementations should use EF Core with proper async/await patterns.
/// </summary>
public interface IGalleryRepository
{
    // ==================== Categories ====================

    /// <summary>
    /// Retrieves all categories, optionally including image counts.
    /// </summary>
    Task<IReadOnlyList<GalleryCategory>> GetAllCategoriesAsync(
        bool includeImageCount = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single category by ID.
    /// </summary>
    Task<GalleryCategory?> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets a category with its images eagerly loaded.
    /// Use sparingly - can be memory-intensive with large galleries.
    /// </summary>
    Task<GalleryCategory?> GetCategoryWithImagesAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Adds a new category to the database.
    /// </summary>
    Task AddCategoryAsync(GalleryCategory category, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    Task UpdateCategoryAsync(GalleryCategory category, CancellationToken ct = default);

    /// <summary>
    /// Deletes a category. Images should be handled separately.
    /// </summary>
    Task DeleteCategoryAsync(GalleryCategory category, CancellationToken ct = default);

    // ==================== Images ====================

    /// <summary>
    /// Retrieves paginated images with optional filtering.
    /// IMPORTANT: Only returns metadata and thumbnails, NOT full image data.
    /// Full image data should be loaded separately to avoid memory issues.
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
    /// Gets the count of images matching the filter criteria.
    /// Used for pagination calculations.
    /// </summary>
    Task<int> GetImagesCountAsync(
        Guid? categoryId = null,
        Guid? tagId = null,
        bool? isFeatured = null,
        bool publicOnly = true,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single image by ID, optionally including tags.
    /// By default, does NOT load full image data - use GetImageDataAsync instead.
    /// </summary>
    Task<GalleryImage?> GetImageByIdAsync(
        Guid id,
        bool includeTags = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets only the binary image data for a specific image.
    /// Use this for serving images to avoid loading full entities.
    /// </summary>
    Task<byte[]?> GetImageDataAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets only the thumbnail binary data for fast list views.
    /// </summary>
    Task<byte[]?> GetThumbnailDataAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets featured images for homepage highlights.
    /// Returns only thumbnails, not full images.
    /// </summary>
    Task<IReadOnlyList<GalleryImage>> GetFeaturedImagesAsync(
        int count = 10,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the most recently uploaded images.
    /// </summary>
    Task<IReadOnlyList<GalleryImage>> GetRecentImagesAsync(
        int count = 20,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new image to the database.
    /// </summary>
    Task AddImageAsync(GalleryImage image, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing image (metadata or binary data).
    /// </summary>
    Task UpdateImageAsync(GalleryImage image, CancellationToken ct = default);

    /// <summary>
    /// Deletes an image from the database.
    /// </summary>
    Task DeleteImageAsync(GalleryImage image, CancellationToken ct = default);

    // ==================== Tags ====================

    /// <summary>
    /// Gets all tags for filtering UI.
    /// </summary>
    Task<IReadOnlyList<GalleryTag>> GetAllTagsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets a tag by name (case-insensitive).
    /// </summary>
    Task<GalleryTag?> GetTagByNameAsync(string name, CancellationToken ct = default);

    /// <summary>
    /// Adds a new tag.
    /// </summary>
    Task AddTagAsync(GalleryTag tag, CancellationToken ct = default);

    // ==================== Unit of Work ====================

    /// <summary>
    /// Persists all pending changes to the database.
    /// Should be called once per request/transaction.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}