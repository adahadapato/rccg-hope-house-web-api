using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Deletes a gallery image record and its associated
/// physical image files.
/// </summary>
public class DeleteGalleryImageCommandHandler
    : IRequestHandler<DeleteGalleryImageCommand, Unit>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryImageStorage _imageStorage;

    public DeleteGalleryImageCommandHandler(
        IGalleryRepository repository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _imageStorage = imageStorage;
    }

    public async Task<Unit> Handle(
        DeleteGalleryImageCommand request,
        CancellationToken ct)
    {
        var image = await _repository.GetImageByIdAsync(
            request.Id,
            includeTags: true,
            ct)
            ?? throw new NotFoundException(
                nameof(GalleryImage),
                request.Id);

        var imagePath =
            image.ImagePath;

        var thumbnailPath =
            image.ThumbnailPath;

        await _repository.DeleteImageAsync(
            image,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        // Delete the files only after the database deletion
        // has succeeded. This avoids leaving a database record
        // pointing to files that have already disappeared.
        try
        {
            await _imageStorage.DeleteAsync(
                imagePath,
                thumbnailPath,
                CancellationToken.None);
        }
        catch
        {
            // The database deletion has succeeded.
            // A physical cleanup failure should not make the
            // completed delete operation appear unsuccessful.
        }

        return Unit.Value;
    }
}