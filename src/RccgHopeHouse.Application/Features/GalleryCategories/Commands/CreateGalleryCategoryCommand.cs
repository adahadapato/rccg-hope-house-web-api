using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

/// <summary>
/// Command used to create a gallery category.
/// </summary>
public record CreateGalleryCategoryCommand(
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<GalleryCategoryDto>;