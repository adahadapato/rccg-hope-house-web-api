using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

/// <summary>
/// Command used to update a gallery category.
/// </summary>
public record UpdateGalleryCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<GalleryCategoryDto>;