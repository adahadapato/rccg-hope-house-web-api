namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Lightweight DTO used when displaying the public gallery feed.
///
/// The feed uses the generated thumbnail where available,
/// avoiding transmission of full-size image data.
/// </summary>
public record GalleryImageFeedDto(
    Guid Id,
    string Title,
    string? ThumbnailPath,
    string ContentType,
    string AltText,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyList<string> Tags,
    bool IsFeatured,
    DateTime? EventDate,
    int DisplayOrder);