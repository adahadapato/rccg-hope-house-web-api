namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a gallery image stored as binary data in the database.
/// Includes metadata for organization, SEO, and accessibility.
/// </summary>
public class GalleryImage : BaseEntity
{
    /// <summary>
    /// Display title for the image (e.g., "Easter Service 2026").
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description or caption for the image.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The actual image binary data (JPEG, PNG, WebP, etc.).
    /// Stored as byte[] for database backup/recovery benefits.
    /// </summary>
    public byte[] ImageData { get; private set; } = Array.Empty<byte>();

    /// <summary>
    /// Compressed thumbnail version for fast list views.
    /// Typically 300x300 pixels or smaller.
    /// </summary>
    public byte[]? ThumbnailData { get; private set; }

    /// <summary>
    /// MIME type of the image (e.g., "image/jpeg", "image/png").
    /// Required for proper HTTP response headers.
    /// </summary>
    public string ContentType { get; private set; } = "image/jpeg";

    /// <summary>
    /// Alternative text for accessibility and SEO.
    /// Describes the image content for screen readers.
    /// </summary>
    public string AltText { get; private set; } = string.Empty;

    /// <summary>
    /// Foreign key to the gallery category (e.g., Services, Events).
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Navigation property to the parent category.
    /// </summary>
    public GalleryCategory Category { get; private set; } = null!;

    /// <summary>
    /// Many-to-many relationship with tags for flexible filtering.
    /// </summary>
    public ICollection<GalleryImageTag> Tags { get; private set; } = new List<GalleryImageTag>();

    /// <summary>
    /// Original file size in bytes (before compression).
    /// Used for display and validation.
    /// </summary>
    public int FileSizeBytes { get; private set; }

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Display order for custom sorting within categories.
    /// Lower numbers appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates if this image should be featured on homepage/highlights.
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Controls public visibility. False = admin only (draft/private).
    /// </summary>
    public bool IsPublic { get; private set; } = true;

    /// <summary>
    /// The date when the photo was taken (event date), not upload date.
    /// Useful for chronological galleries.
    /// </summary>
    public DateTime? EventDate { get; private set; }

    /// <summary>
    /// Name of the photographer (optional attribution).
    /// </summary>
    public string? Photographer { get; private set; }

    /// <summary>
    /// Number of times this image has been viewed.
    /// Updated asynchronously to avoid performance impact.
    /// </summary>
    public int ViewCount { get; private set; }

    /// <summary>
    /// SHA256 hash of the image data for duplicate detection.
    /// Prevents storing the same image multiple times.
    /// </summary>
    public string? ImageHash { get; private set; }

    /// <summary>
    /// EF Core requires a parameterless constructor for entity materialization.
    /// </summary>
    private GalleryImage() { }

    /// <summary>
    /// Factory method to create a new gallery image with validation.
    /// Enforces domain invariants and business rules.
    /// </summary>
    /// <param name="title">Display title (required, max 200 chars)</param>
    /// <param name="imageData">Binary image data (required, max 10MB)</param>
    /// <param name="thumbnailData">Optional pre-generated thumbnail</param>
    /// <param name="categoryId">Parent category ID (required)</param>
    /// <param name="altText">Accessibility alt text (required)</param>
    /// <param name="contentType">MIME type (default: image/jpeg)</param>
    /// <param name="fileSizeBytes">Original file size in bytes</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="description">Optional description</param>
    /// <param name="eventDate">Optional event date</param>
    /// <param name="photographer">Optional photographer name</param>
    /// <returns>New GalleryImage instance</returns>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid</exception>
    public static GalleryImage Create(
        string title,
        byte[] imageData,
        Guid categoryId,
        string altText,
        string contentType = "image/jpeg",
        int fileSizeBytes = 0,
        int width = 0,
        int height = 0,
        byte[]? thumbnailData = null,
        string? description = null,
        DateTime? eventDate = null,
        string? photographer = null)
    {
        // Validate required fields using .NET 7+ argument validation
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(altText, nameof(altText));

        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty.", nameof(imageData));

        // Enforce 10MB limit to prevent database bloat
        if (imageData.Length > 10 * 1024 * 1024)
            throw new ArgumentException("Image size cannot exceed 10MB.", nameof(imageData));

        return new GalleryImage
        {
            Title = title.Trim(),
            ImageData = imageData,
            ThumbnailData = thumbnailData,
            CategoryId = categoryId,
            AltText = altText.Trim(),
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes > 0 ? fileSizeBytes : imageData.Length,
            Width = width,
            Height = height,
            Description = description?.Trim(),
            EventDate = eventDate,
            Photographer = photographer?.Trim(),
            IsFeatured = false,
            IsPublic = true,
            DisplayOrder = 0,
            ViewCount = 0,
            // Generate hash for duplicate detection
            ImageHash = GenerateHash(imageData)
        };
    }

    /// <summary>
    /// Updates image metadata (title, description, alt text, photographer).
    /// Does NOT modify the actual image binary data.
    /// </summary>
    public void UpdateMetadata(string title, string? description, string altText, string? photographer)
    {
        Title = title.Trim();
        Description = description?.Trim();
        AltText = altText.Trim();
        Photographer = photographer?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Replaces the image binary data and regenerates the hash.
    /// Should be called when uploading a new version of the image.
    /// </summary>
    public void ReplaceImage(byte[] imageData, byte[]? thumbnailData = null)
    {
        if (imageData.Length == 0)
            throw new ArgumentException("Image data cannot be empty.", nameof(imageData));

        if (imageData.Length > 10 * 1024 * 1024)
            throw new ArgumentException("Image size cannot exceed 10MB.", nameof(imageData));

        ImageData = imageData;
        ThumbnailData = thumbnailData;
        FileSizeBytes = imageData.Length;
        ImageHash = GenerateHash(imageData);
        MarkAsUpdated();
    }

    /// <summary>
    /// Marks the image as featured (appears in homepage highlights).
    /// </summary>
    public void SetFeatured(bool isFeatured) => IsFeatured = isFeatured;

    /// <summary>
    /// Toggles public visibility. Private images are admin-only.
    /// </summary>
    public void TogglePublic() => IsPublic = !IsPublic;

    /// <summary>
    /// Increments the view counter.
    /// Should be called asynchronously to avoid blocking page loads.
    /// </summary>
    public void IncrementViewCount() => ViewCount++;

    /// <summary>
    /// Sets the display order for custom sorting.
    /// </summary>
    public void SetDisplayOrder(int order) => DisplayOrder = order;

    /// <summary>
    /// Adds a tag to the image if not already present.
    /// </summary>
    public void AddTag(Guid tagId)
    {
        if (!Tags.Any(t => t.TagId == tagId))
        {
            Tags.Add(new GalleryImageTag { ImageId = Id, TagId = tagId });
        }
    }

    /// <summary>
    /// Removes a tag from the image.
    /// </summary>
    public void RemoveTag(Guid tagId)
    {
        var tagToRemove = Tags.FirstOrDefault(t => t.TagId == tagId);
        if (tagToRemove != null)
        {
            Tags.Remove(tagToRemove);
        }
    }

    /// <summary>
    /// Generates a SHA256 hash of the image data for duplicate detection.
    /// </summary>
    private static string GenerateHash(byte[] data)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(data);
        return Convert.ToHexString(hash);
    }
}
