using System.Security.Cryptography;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a gallery image whose physical files are stored
/// outside the database.
///
/// The database stores the image paths together with metadata used
/// for organisation, accessibility, filtering and presentation.
/// </summary>
public class GalleryImage : BaseEntity
{
    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string ImagePath { get; private set; } = string.Empty;

    public string? ThumbnailPath { get; private set; }

    public string ContentType { get; private set; } = "image/webp";

    public string AltText { get; private set; } = string.Empty;

    public Guid CategoryId { get; private set; }

    public GalleryCategory Category { get; private set; } = null!;

    public ICollection<GalleryImageTag> Tags { get; private set; }
        = new List<GalleryImageTag>();

    public int FileSizeBytes { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    /// <summary>
    /// Controls custom ordering within the gallery.
    /// Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    public bool IsFeatured { get; private set; }

    public bool IsPublic { get; private set; } = true;

    public DateTime? EventDate { get; private set; }

    public string? Photographer { get; private set; }

    public int ViewCount { get; private set; }

    public string? ImageHash { get; private set; }

    private GalleryImage()
    {
    }

    public static GalleryImage Create(
        string title,
        string imagePath,
        Guid categoryId,
        string altText,
        string contentType,
        int fileSizeBytes,
        int width,
        int height,
        string imageHash,
        string? thumbnailPath = null,
        string? description = null,
        DateTime? eventDate = null,
        string? photographer = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            imagePath,
            nameof(imagePath));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            altText,
            nameof(altText));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            contentType,
            nameof(contentType));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            imageHash,
            nameof(imageHash));

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid gallery category is required.",
                nameof(categoryId));
        }

        if (fileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                "Image file size must be greater than zero.");
        }

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Image width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Image height must be greater than zero.");
        }

        return new GalleryImage
        {
            Title = title.Trim(),
            Description = description?.Trim(),

            ImagePath = imagePath.Trim(),
            ThumbnailPath = thumbnailPath?.Trim(),

            ContentType = contentType.Trim(),
            AltText = altText.Trim(),

            CategoryId = categoryId,

            FileSizeBytes = fileSizeBytes,
            Width = width,
            Height = height,

            DisplayOrder = 0,
            IsFeatured = false,
            IsPublic = true,

            EventDate = eventDate,
            Photographer = photographer?.Trim(),

            ViewCount = 0,

            ImageHash = imageHash.Trim()
        };
    }

    /// <summary>
    /// Updates editable textual metadata.
    /// </summary>
    public void UpdateMetadata(
        string title,
        string? description,
        string altText,
        string? photographer,
        DateTime? eventDate = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            altText,
            nameof(altText));

        Title = title.Trim();
        Description = description?.Trim();
        AltText = altText.Trim();
        Photographer = photographer?.Trim();
        EventDate = eventDate;

        MarkAsUpdated();
    }

    /// <summary>
    /// Changes the gallery category assigned to this image.
    /// Category existence and availability are validated by
    /// the Application layer before this method is called.
    /// </summary>
    public void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid gallery category is required.",
                nameof(categoryId));
        }

        if (CategoryId == categoryId)
        {
            return;
        }

        CategoryId = categoryId;

        MarkAsUpdated();
    }

    /// <summary>
    /// Replaces the stored image information after the physical
    /// image file has been replaced successfully.
    /// </summary>
    public void ReplaceImage(
        string imagePath,
        string? thumbnailPath,
        string contentType,
        int fileSizeBytes,
        int width,
        int height,
        string imageHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            imagePath,
            nameof(imagePath));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            contentType,
            nameof(contentType));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            imageHash,
            nameof(imageHash));

        if (fileSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes),
                "Image file size must be greater than zero.");
        }

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(width),
                "Image width must be greater than zero.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(height),
                "Image height must be greater than zero.");
        }

        ImagePath = imagePath.Trim();
        ThumbnailPath = thumbnailPath?.Trim();

        ContentType = contentType.Trim();

        FileSizeBytes = fileSizeBytes;
        Width = width;
        Height = height;

        ImageHash = imageHash.Trim();

        MarkAsUpdated();
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        MarkAsUpdated();
    }

    public void TogglePublic()
    {
        IsPublic = !IsPublic;
        MarkAsUpdated();
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    /// <summary>
    /// Sets custom display order.
    /// Lower values appear first.
    /// </summary>
    public void SetDisplayOrder(int order)
    {
        if (order < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(order),
                "Display order cannot be negative.");
        }

        if (DisplayOrder == order)
        {
            return;
        }

        DisplayOrder = order;
        MarkAsUpdated();
    }

    public void AddTag(Guid tagId)
    {
        if (tagId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid tag ID is required.",
                nameof(tagId));
        }

        if (Tags.Any(
                tag => tag.TagId == tagId))
        {
            return;
        }

        Tags.Add(
            new GalleryImageTag
            {
                ImageId = Id,
                TagId = tagId
            });
    }

    public void RemoveTag(Guid tagId)
    {
        var tagToRemove =
            Tags.FirstOrDefault(
                tag =>
                    tag.TagId == tagId);

        if (tagToRemove is not null)
        {
            Tags.Remove(tagToRemove);
        }
    }

    public static string GenerateImageHash(
        byte[] imageData)
    {
        ArgumentNullException.ThrowIfNull(
            imageData);

        if (imageData.Length == 0)
        {
            throw new ArgumentException(
                "Image data cannot be empty.",
                nameof(imageData));
        }

        var hash =
            SHA256.HashData(
                imageData);

        return Convert.ToHexString(
            hash);
    }
}