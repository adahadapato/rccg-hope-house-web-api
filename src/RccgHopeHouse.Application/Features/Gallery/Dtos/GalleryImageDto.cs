namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Detailed DTO for admin views and single-image public requests.
/// Includes full binary image data, metadata, and tag information.
/// </summary>
public record GalleryImageDto(
    Guid Id,
    string Title,
    string? Description,
    byte[] ImageData,
    byte[]? ThumbnailData,
    string ContentType,
    string AltText,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyList<string> Tags,
    int FileSizeBytes,
    int Width,
    int Height,
    int DisplayOrder,
    bool IsFeatured,
    bool IsPublic,
    DateTime? EventDate,
    string? Photographer,
    int ViewCount,
    DateTime CreatedAt);