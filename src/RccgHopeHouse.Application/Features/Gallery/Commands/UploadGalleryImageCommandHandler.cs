using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Handles uploading and persistence of gallery images.
/// </summary>
public class UploadGalleryImageCommandHandler
    : IRequestHandler<UploadGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;

    public UploadGalleryImageCommandHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GalleryImageDto> Handle(
        UploadGalleryImageCommand request,
        CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(
            request.CategoryId,
            ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.CategoryId);

        var image = GalleryImage.Create(
            title: request.Title,
            imageData: request.ImageData,
            categoryId: request.CategoryId,
            altText: request.AltText,
            contentType: request.ContentType,
            fileSizeBytes: request.ImageData.Length,
            width: 0,
            height: 0,
            description: request.Description,
            eventDate: request.EventDate,
            photographer: request.Photographer);

        if (request.Tags?.Any() == true)
        {
            foreach (var tagName in request.Tags)
            {
                var tag = await _repository.GetTagByNameAsync(
                    tagName,
                    ct);

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

        return await MapToDetailDtoAsync(
            image,
            category.Name,
            ct);
    }

    private async Task<GalleryImageDto> MapToDetailDtoAsync(
        GalleryImage image,
        string categoryName,
        CancellationToken ct)
    {
        var tags = await _repository.GetAllTagsAsync(ct);

        var tagNames = image.Tags
            .Select(t => tags
                .First(tg => tg.Id == t.TagId)
                .Name)
            .ToList();

        return new GalleryImageDto(
            image.Id,
            image.Title,
            image.Description,
            image.ImageData,
            image.ThumbnailData,
            image.ContentType,
            image.AltText,
            image.CategoryId,
            categoryName,
            tagNames,
            image.FileSizeBytes,
            image.Width,
            image.Height,
            image.DisplayOrder,
            image.IsFeatured,
            image.IsPublic,
            image.EventDate,
            image.Photographer,
            image.ViewCount,
            image.CreatedAt);
    }
}