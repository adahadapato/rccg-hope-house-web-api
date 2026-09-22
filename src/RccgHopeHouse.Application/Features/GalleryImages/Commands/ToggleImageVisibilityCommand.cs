using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public record ToggleImageVisibilityCommand(Guid Id) : IRequest<GalleryImageDto>;