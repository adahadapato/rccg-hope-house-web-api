namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Lightweight DTO used when displaying the public gallery feed.
///
/// The feed uses the generated thumbnail where available,
/// avoiding transmission of full-size image data.
///
/// Width and Height are included so the frontend can preserve
/// each photograph's natural aspect ratio without loading the
/// full-size image first.
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
    int Width,
    int Height,
    bool IsFeatured,
    DateTime? EventDate,
    int DisplayOrder);