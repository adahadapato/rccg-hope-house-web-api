using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Dtos;

/// <summary>
/// Represents a gallery category available within the application.
/// </summary>
public record GalleryCategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int DisplayOrder)
{
    /// <summary>
    /// Creates a DTO from a GalleryCategory domain entity.
    /// </summary>
    public static GalleryCategoryDto FromEntity(
        GalleryCategory category) => new(
            Id: category.Id,
            Name: category.Name,
            Description: category.Description,
            IsActive: category.IsActive,
            DisplayOrder: category.DisplayOrder);
}