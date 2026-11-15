using MediatR;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Announcements.Commands;

/// <summary>
/// Uploads (or replaces) the poster/flyer for an announcement.
/// </summary>
public record SetAnnouncementImageCommand(
    Guid Id,
    byte[] ImageData,
    string ContentType)
    : IRequest<AnnouncementDto>;

public class SetAnnouncementImageCommandHandler
    : IRequestHandler<SetAnnouncementImageCommand, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;
    private readonly IGalleryImageStorage _imageStorage;

    public SetAnnouncementImageCommandHandler(
        IAnnouncementRepository repository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _imageStorage = imageStorage;
    }

    public async Task<AnnouncementDto> Handle(
        SetAnnouncementImageCommand request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        var previousImage = announcement.ImagePath;
        var previousThumbnail = announcement.ThumbnailPath;

        var stored = await _imageStorage.SaveAsync(
            request.ImageData,
            request.ContentType,
            ct);

        try
        {
            announcement.SetImage(stored.ImagePath, stored.ThumbnailPath);

            await _repository.UpdateAsync(announcement, ct);
            await _repository.SaveChangesAsync(ct);
        }
        catch
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    stored.ImagePath,
                    stored.ThumbnailPath,
                    CancellationToken.None);
            }
            catch
            {
                // Preserve the original exception.
            }

            throw;
        }

        // Remove the old poster only after the database update succeeded.
        if (previousImage is not null || previousThumbnail is not null)
        {
            await _imageStorage.DeleteAsync(
                previousImage,
                previousThumbnail,
                CancellationToken.None);
        }

        return announcement.ToDto();
    }
}

public record RemoveAnnouncementImageCommand(Guid Id)
    : IRequest<AnnouncementDto>;

public class RemoveAnnouncementImageCommandHandler
    : IRequestHandler<RemoveAnnouncementImageCommand, AnnouncementDto>
{
    private readonly IAnnouncementRepository _repository;
    private readonly IGalleryImageStorage _imageStorage;

    public RemoveAnnouncementImageCommandHandler(
        IAnnouncementRepository repository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _imageStorage = imageStorage;
    }

    public async Task<AnnouncementDto> Handle(
        RemoveAnnouncementImageCommand request,
        CancellationToken ct)
    {
        var announcement =
            await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Announcement), request.Id);

        var imagePath = announcement.ImagePath;
        var thumbnailPath = announcement.ThumbnailPath;

        if (imagePath is null && thumbnailPath is null)
        {
            return announcement.ToDto();
        }

        announcement.ClearImage();

        await _repository.UpdateAsync(announcement, ct);
        await _repository.SaveChangesAsync(ct);

        await _imageStorage.DeleteAsync(imagePath, thumbnailPath, CancellationToken.None);

        return announcement.ToDto();
    }
}
