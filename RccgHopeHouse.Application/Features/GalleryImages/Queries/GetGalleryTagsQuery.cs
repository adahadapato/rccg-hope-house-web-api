using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public record GetGalleryTagsQuery : IRequest<IReadOnlyList<GalleryTagDto>>;