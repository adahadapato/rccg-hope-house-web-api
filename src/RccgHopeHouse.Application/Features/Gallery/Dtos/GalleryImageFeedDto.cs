namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Lightweight DTO for public gallery grid/list views.
/// Excludes heavy full-resolution binary data to optimize bandwidth and memory.
/// Only includes thumbnail data, metadata, and category/tag names for filtering.
/// </summary>
public record GalleryImageFeedDto(
    Guid Id,
    string Title,
    byte[]? ThumbnailData,
    string ContentType,
    string AltText,
    Guid CategoryId,
    string CategoryName,
    IReadOnlyList<string> Tags,
    bool IsFeatured,
    DateTime? EventDate,
    int DisplayOrder);