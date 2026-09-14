using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public class DeleteGalleryImageCommandHandler : IRequestHandler<DeleteGalleryImageCommand, Unit>
{
    private readonly IGalleryRepository _repository;

    public DeleteGalleryImageCommandHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeleteGalleryImageCommand request, CancellationToken ct)
    {
        var image = await _repository.GetImageByIdAsync(request.Id, true, ct)
            ?? throw new NotFoundException(nameof(GalleryImage), request.Id);

        await _repository.DeleteImageAsync(image, ct);
        await _repository.SaveChangesAsync(ct);
        return Unit.Value;
    }
}