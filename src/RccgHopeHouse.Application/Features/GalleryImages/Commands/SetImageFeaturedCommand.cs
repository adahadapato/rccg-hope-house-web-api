using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public record SetImageFeaturedCommand(Guid Id, bool IsFeatured) : IRequest<GalleryImageDto>;