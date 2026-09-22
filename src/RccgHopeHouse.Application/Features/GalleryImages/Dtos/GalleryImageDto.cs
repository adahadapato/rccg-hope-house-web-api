namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Detailed gallery image DTO used for admin views
/// and single-image requests.
///
/// Physical image files are stored outside SQL Server.
/// ImagePath and ThumbnailPath identify the corresponding
/// files served by the application.
/// </summary>
public record GalleryImageDto(
    Guid Id,
    string Title,
    string? Description,
    string ImagePath,
    string? ThumbnailPath,
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