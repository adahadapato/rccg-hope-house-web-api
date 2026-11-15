namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a tag for flexible image categorization.
/// Tags are many-to-many with images (e.g., "Easter", "Youth", "2026").
/// </summary>
public class GalleryTag : BaseEntity
{
    /// <summary>
    /// Tag name (stored lowercase for case-insensitive matching).
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description of what this tag represents.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Join table collection linking this tag to images.
    /// </summary>
    public ICollection<GalleryImageTag> Images { get; private set; } = new List<GalleryImageTag>();

    /// <summary>
    /// EF Core parameterless constructor.
    /// </summary>
    private GalleryTag() { }

    /// <summary>
    /// Factory method to create a tag.
    /// </summary>
    public static GalleryTag Create(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GalleryTag
        {
            Name = name.Trim().ToLowerInvariant(), // Normalize to lowercase
            Description = description?.Trim()
        };
    }
}