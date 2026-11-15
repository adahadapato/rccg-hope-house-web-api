namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Join entity for many-to-many relationship between images and tags.
/// Configured with composite primary key (ImageId + TagId).
/// </summary>
public class GalleryImageTag
{
    /// <summary>
    /// Foreign key to GalleryImage.
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// Navigation property to the image.
    /// </summary>
    public GalleryImage Image { get; set; } = null!;

    /// <summary>
    /// Foreign key to GalleryTag.
    /// </summary>
    public Guid TagId { get; set; }

    /// <summary>
    /// Navigation property to the tag.
    /// </summary>
    public GalleryTag Tag { get; set; } = null!;
}