using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Handles gallery image uploads.
///
/// The physical image and thumbnail are processed and stored
/// through IGalleryImageStorage. SQL Server stores only the
/// resulting paths and image metadata.
/// </summary>
public class UploadGalleryImageCommandHandler
    : IRequestHandler<UploadGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;
    private readonly IGalleryImageStorage _imageStorage;

    public UploadGalleryImageCommandHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _imageStorage = imageStorage;
    }

    public async Task<GalleryImageDto> Handle(
        UploadGalleryImageCommand request,
        CancellationToken ct)
    {
        if (request.DisplayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.DisplayOrder),
                "Display order cannot be negative.");
        }

        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.CategoryId);

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "Images cannot be uploaded to an inactive gallery category.");
        }

        var storedImage =
            await _imageStorage.SaveAsync(
                request.ImageData,
                request.ContentType,
                ct);

        GalleryImage image;

        try
        {
            image = GalleryImage.Create(
                title: request.Title,
                imagePath: storedImage.ImagePath,
                categoryId: request.CategoryId,
                altText: request.AltText,
                contentType: storedImage.ContentType,
                fileSizeBytes: storedImage.FileSizeBytes,
                width: storedImage.Width,
                height: storedImage.Height,
                imageHash: storedImage.ImageHash,
                thumbnailPath: storedImage.ThumbnailPath,
                description: request.Description,
                eventDate: request.EventDate,
                photographer: request.Photographer);

            image.SetDisplayOrder(
                request.DisplayOrder);

            var requestedTagNames =
                NormalizeTagNames(
                    request.Tags);

            foreach (var tagName in requestedTagNames)
            {
                var tag =
                    await _repository.GetTagByNameAsync(
                        tagName,
                        ct);

                if (tag is null)
                {
                    tag =
                        GalleryTag.Create(
                            tagName);

                    await _repository.AddTagAsync(
                        tag,
                        ct);
                }

                image.AddTag(
                    tag.Id);
            }

            await _repository.AddImageAsync(
                image,
                ct);

            await _repository.SaveChangesAsync(
                ct);
        }
        catch
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    storedImage.ImagePath,
                    storedImage.ThumbnailPath,
                    CancellationToken.None);
            }
            catch
            {
                // Preserve the original exception.
                // Orphaned files can be removed by maintenance.
            }

            throw;
        }

        // Mapping occurs after persistence has succeeded.
        // A mapping failure must never cause successfully stored
        // physical files to be deleted.
        return await MapToDetailDtoAsync(
            image,
            category.Name,
            ct);
    }

    private static IReadOnlyList<string> NormalizeTagNames(
        IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return Array.Empty<string>();
        }

        return tags
            .Where(tag =>
                !string.IsNullOrWhiteSpace(tag))
            .Select(tag =>
                tag.Trim())
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<GalleryImageDto> MapToDetailDtoAsync(
        GalleryImage image,
        string categoryName,
        CancellationToken ct)
    {
        var tags =
            await _repository.GetAllTagsAsync(
                ct);

        var tagNames =
            image.Tags
                .Select(imageTag =>
                    tags.FirstOrDefault(
                        tag =>
                            tag.Id ==
                            imageTag.TagId)
                        ?.Name)
                .Where(name =>
                    !string.IsNullOrWhiteSpace(
                        name))
                .Select(name => name!)
                .ToList();

        return new GalleryImageDto(
            image.Id,
            image.Title,
            image.Description,
            image.ImagePath,
            image.ThumbnailPath,
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