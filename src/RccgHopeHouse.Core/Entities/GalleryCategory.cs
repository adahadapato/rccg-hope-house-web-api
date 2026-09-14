namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a category for organizing gallery images.
/// Examples: "Sunday Services", "Youth Events", "Outreach Programs".
/// </summary>
public class GalleryCategory : BaseEntity
{
    /// <summary>
    /// Category name (e.g., "Sunday Services").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description of the category.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Optional cover image (stored as byte[] like other images).
    /// Used for category thumbnails in gallery views.
    /// </summary>
    public byte[]? CoverImageData { get; private set; }

    /// <summary>
    /// Display order for sorting categories.
    /// Lower numbers appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Indicates if the category is active and visible.
    /// Inactive categories are hidden from public views.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Navigation property to images in this category.
    /// Configured as one-to-many relationship.
    /// </summary>
    public ICollection<GalleryImage> Images { get; private set; } = new List<GalleryImage>();

    /// <summary>
    /// EF Core parameterless constructor for entity materialization.
    /// </summary>
    private GalleryCategory() { }

    /// <summary>
    /// Factory method to create a new category.
    /// </summary>
    /// <param name="name">Category name (required, max 100 chars)</param>
    /// <param name="description">Optional description</param>
    /// <param name="displayOrder">Sort order (default: 0)</param>
    /// <returns>New GalleryCategory instance</returns>
    public static GalleryCategory Create(string name, string? description = null, int displayOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GalleryCategory
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    /// <summary>
    /// Updates category metadata.
    /// </summary>
    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets or updates the cover image binary data.
    /// </summary>
    public void SetCoverImage(byte[]? coverImageData)
    {
        CoverImageData = coverImageData;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activates or deactivates the category.
    /// </summary>
    public void ToggleActive() => IsActive = !IsActive;
}