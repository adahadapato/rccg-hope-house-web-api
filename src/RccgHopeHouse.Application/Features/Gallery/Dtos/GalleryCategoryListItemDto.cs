namespace RccgHopeHouse.Application.Features.Gallery.Dtos;

/// <summary>
/// Lightweight DTO for category dropdowns and admin lists.
/// Excludes binary data and navigation properties to optimize query performance.
/// </summary>
public record GalleryCategoryListItemDto(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    int ImageCount = 0); // Optional: populated via separate query if needed