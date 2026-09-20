namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a category used to organise gallery images within
/// RCCG Hope House.
///
/// Examples include Sunday Services, Youth Events,
/// Member Celebrations and Outreach & Evangelism.
///
/// Gallery categories are stored as entities rather than enums so that
/// authorised administrators can add, update, activate, deactivate
/// and reorder categories without requiring application code changes.
/// </summary>
public class GalleryCategory : BaseEntity
{
    // ==================== Properties ====================

    /// <summary>
    /// The display name of the gallery category.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description explaining the purpose of the category.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Optional cover image used as the category thumbnail.
    /// </summary>
    public byte[]? CoverImageData { get; private set; }

    /// <summary>
    /// Determines the order in which the category appears.
    /// Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Determines whether the category is currently available
    /// for public gallery use.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Images belonging to this gallery category.
    /// </summary>
    public ICollection<GalleryImage> Images { get; private set; }
        = new List<GalleryImage>();

    // ==================== Constructor ====================

    /// <summary>
    /// Parameterless constructor required by Entity Framework Core.
    /// </summary>
    private GalleryCategory()
    {
    }

    // ==================== Factory ====================

    /// <summary>
    /// Creates a new gallery category.
    /// </summary>
    public static GalleryCategory Create(
        string name,
        int displayOrder,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        return new GalleryCategory
        {
            Name = name.Trim(),
            Description = NormalizeOptionalText(description),
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    // ==================== Update ====================

    /// <summary>
    /// Updates the editable category details.
    /// </summary>
    public void Update(
        string name,
        string? description,
        int displayOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = name.Trim();
        Description = NormalizeOptionalText(description);
        DisplayOrder = displayOrder;

        MarkAsUpdated();
    }

    // ==================== Cover Image ====================

    /// <summary>
    /// Sets or removes the cover image for the category.
    /// </summary>
    public void SetCoverImage(byte[]? coverImageData)
    {
        CoverImageData = coverImageData;
        MarkAsUpdated();
    }

    // ==================== Activation ====================

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        MarkAsUpdated();
    }

    // ==================== Display Order ====================

    public void ChangeDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        if (DisplayOrder == displayOrder)
            return;

        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }

    // ==================== Helpers ====================

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}