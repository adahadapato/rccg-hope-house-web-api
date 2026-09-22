using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public record GetGalleryImageByIdQuery(Guid Id, bool PublicOnly = true) : IRequest<GalleryImageDto>;