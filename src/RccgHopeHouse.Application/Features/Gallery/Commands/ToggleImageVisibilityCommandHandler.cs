using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public class ToggleImageVisibilityCommandHandler : IRequestHandler<ToggleImageVisibilityCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;

    public ToggleImageVisibilityCommandHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<GalleryImageDto> Handle(ToggleImageVisibilityCommand request, CancellationToken ct)
    {
        var image = await _repository.GetImageByIdAsync(request.Id, true, ct)
            ?? throw new NotFoundException(nameof(GalleryImage), request.Id);

        image.TogglePublic();
        await _repository.UpdateImageAsync(image, ct);
        await _repository.SaveChangesAsync(ct);

        var category = await _repository.GetCategoryByIdAsync(image.CategoryId, ct);
        return new GalleryImageDto(
            image.Id, image.Title, image.Description, image.ImageData, image.ThumbnailData,
            image.ContentType, image.AltText, image.CategoryId, category?.Name ?? "Uncategorized",
            Array.Empty<string>(), image.FileSizeBytes, image.Width, image.Height, image.DisplayOrder,
            image.IsFeatured, image.IsPublic, image.EventDate, image.Photographer, image.ViewCount, image.CreatedAt);
    }
}