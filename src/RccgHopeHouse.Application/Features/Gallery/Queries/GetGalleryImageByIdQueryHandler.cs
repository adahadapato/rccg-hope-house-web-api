using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public class GetGalleryImageByIdQueryHandler : IRequestHandler<GetGalleryImageByIdQuery, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;

    public GetGalleryImageByIdQueryHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<GalleryImageDto> Handle(GetGalleryImageByIdQuery request, CancellationToken ct)
    {
        var image = request.PublicOnly
            ? await _repository.GetImageByIdAsync(request.Id, includeTags: true, ct)
            : await _repository.GetImageByIdAsync(request.Id, includeTags: true, ct); // Adjust repo method if admin needs drafts

        if (image == null || (request.PublicOnly && !image.IsPublic))
            throw new NotFoundException(nameof(GalleryImage), request.Id);

        // Fire-and-forget view count increment
        _ = Task.Run(async () =>
        {
            image.IncrementViewCount();
            await _repository.UpdateImageAsync(image, CancellationToken.None);
            await _repository.SaveChangesAsync(CancellationToken.None);
        }, ct);

        var category = await _repository.GetCategoryByIdAsync(image.CategoryId, ct);
        var tagNames = image.Tags.Select(t => t.Tag.Name).ToList();

        return new GalleryImageDto(
            image.Id, image.Title, image.Description, image.ImageData, image.ThumbnailData,
            image.ContentType, image.AltText, image.CategoryId, category?.Name ?? "Uncategorized",
            tagNames, image.FileSizeBytes, image.Width, image.Height, image.DisplayOrder,
            image.IsFeatured, image.IsPublic, image.EventDate, image.Photographer, image.ViewCount, image.CreatedAt);
    }
}