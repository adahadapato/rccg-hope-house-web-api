using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public class UpdateGalleryImageCommandHandler
    : IRequestHandler<UpdateGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;

    public UpdateGalleryImageCommandHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GalleryImageDto> Handle(
        UpdateGalleryImageCommand request,
        CancellationToken ct)
    {
        var image = await _repository.GetImageByIdAsync(
            request.Id,
            includeTags: true,
            ct)
            ?? throw new NotFoundException(
                nameof(GalleryImage),
                request.Id);

        image.UpdateMetadata(
            request.Title,
            request.Description,
            request.AltText,
            request.Photographer);

        if (request.NewImageData is not null &&
            !string.IsNullOrWhiteSpace(request.NewContentType))
        {
            image.ReplaceImage(request.NewImageData);
        }

        await _repository.UpdateImageAsync(image, ct);
        await _repository.SaveChangesAsync(ct);

        return await MapToDetailDtoAsync(image, ct);
    }

    private async Task<GalleryImageDto> MapToDetailDtoAsync(
        GalleryImage image,
        CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(
            image.CategoryId,
            ct);

        var tagNames = image.Tags
            .Select(t => t.Tag.Name)
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
            category?.Name ?? "Uncategorized",
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