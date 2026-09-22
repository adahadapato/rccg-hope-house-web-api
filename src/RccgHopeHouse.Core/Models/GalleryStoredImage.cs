namespace RccgHopeHouse.Core.Models;

/// <summary>
/// Describes a gallery image after it has been processed
/// and saved to physical storage.
/// </summary>
public sealed record GalleryStoredImage(
    string ImagePath,
    string ThumbnailPath,
    string ContentType,
    int FileSizeBytes,
    int Width,
    int Height,
    string ImageHash);