using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Commands;

public record DeleteAnnouncementCommand(Guid Id) : IRequest;

public class DeleteAnnouncementCommandHandler
    : IRequestHandler<DeleteAnnouncementCommand>
{
    private readonly IAnnouncementRepository _repository;
    private readonly IGalleryImageStorage _imageStorage;

    public DeleteAnnouncementCommandHandler(
        IAnnouncementRepository repository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _imageStorage = imageStorage;
    }

    public async Task Handle(
        DeleteAnnouncementCommand request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        var imagePath = announcement.ImagePath;
        var thumbnailPath = announcement.ThumbnailPath;

        await _repository.DeleteAsync(announcement, ct);
        await _repository.SaveChangesAsync(ct);

        // Only remove the files once the record is gone.
        if (imagePath is not null || thumbnailPath is not null)
        {
            await _imageStorage.DeleteAsync(imagePath, thumbnailPath, CancellationToken.None);
        }
    }
}
