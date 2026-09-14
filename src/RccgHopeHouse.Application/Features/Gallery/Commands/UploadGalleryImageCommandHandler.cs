using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Handler for uploading a gallery image.
/// Orchestrates domain creation, tag association, and persistence.
/// Note: Actual image resizing/thumbnail generation occurs in Infrastructure via IImageStorageService or ImageSharp.
/// </summary>
public class UploadGalleryImageCommandHandler : IRequestHandler<UploadGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;

    public UploadGalleryImageCommandHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<GalleryImageDto> Handle(UploadGalleryImageCommand request, CancellationToken ct)
    {
        // Domain factory creates entity with validation
        var image = GalleryImage.Create(
            title: request.Title,
            imageData: request.ImageData,
            categoryId: request.CategoryId,
            altText: request.AltText,
            contentType: request.ContentType,
            fileSizeBytes: request.ImageData.Length,
            width: 0, // Infrastructure will update dimensions after processing
            height: 0,
            description: request.Description,
            eventDate: request.EventDate,
            photographer: request.Photographer);

        // Associate tags if provided
        if (request.Tags?.Any() == true)
        {
            foreach (var tagName in request.Tags)
            {
                var tag = await _repository.GetTagByNameAsync(tagName, ct);
                if (tag == null)
                {
                    tag = GalleryTag.Create(tagName);
                    await _repository.AddTagAsync(tag, ct);
                }
                image.AddTag(tag.Id);
            }
        }

        await _repository.AddImageAsync(image, ct);
        await _repository.SaveChangesAsync(ct);

        return await MapToDetailDtoAsync(image, ct);
    }

    private async Task<GalleryImageDto> MapToDetailDtoAsync(GalleryImage image, CancellationToken ct)
    {
        var category = await _repository.GetCategoryByIdAsync(image.CategoryId, ct);
        var tags = await _repository.GetAllTagsAsync(ct); // Simplified; optimize in Infrastructure
        var tagNames = image.Tags.Select(t => tags.First(tg => tg.Id == t.TagId).Name).ToList();

        return new GalleryImageDto(
            image.Id, image.Title, image.Description, image.ImageData, image.ThumbnailData,
            image.ContentType, image.AltText, image.CategoryId, category?.Name ?? "Uncategorized",
            tagNames, image.FileSizeBytes, image.Width, image.Height, image.DisplayOrder,
            image.IsFeatured, image.IsPublic, image.EventDate, image.Photographer, image.ViewCount, image.CreatedAt);
    }
}