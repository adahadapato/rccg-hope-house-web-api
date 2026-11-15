using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

public record ActivateGalleryCategoryCommand(
    Guid Id) : IRequest<GalleryCategoryDto>;