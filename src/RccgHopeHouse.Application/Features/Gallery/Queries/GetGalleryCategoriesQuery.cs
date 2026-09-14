using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public record GetGalleryCategoriesQuery(bool IncludeInactive = false) : IRequest<IReadOnlyList<GalleryCategoryDto>>;