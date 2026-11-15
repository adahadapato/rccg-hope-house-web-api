using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Queries;

public record GetAllGalleryCategoriesQuery()
    : IRequest<IReadOnlyList<GalleryCategoryDto>>;