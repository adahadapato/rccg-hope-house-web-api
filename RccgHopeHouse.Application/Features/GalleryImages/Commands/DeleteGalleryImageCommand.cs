using MediatR;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public record DeleteGalleryImageCommand(Guid Id) : IRequest<Unit>;