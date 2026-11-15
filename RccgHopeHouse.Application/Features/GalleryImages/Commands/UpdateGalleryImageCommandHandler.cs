using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Models;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

/// <summary>
/// Handles updates to gallery image metadata, organisation,
/// tags, display order and optional replacement of the
/// physical image.
/// </summary>
public class UpdateGalleryImageCommandHandler
    : IRequestHandler<UpdateGalleryImageCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;
    private readonly IGalleryImageStorage _imageStorage;

    public UpdateGalleryImageCommandHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository,
        IGalleryImageStorage imageStorage)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _imageStorage = imageStorage;
    }

    public async Task<GalleryImageDto> Handle(
        UpdateGalleryImageCommand request,
        CancellationToken ct)
    {
        if (request.DisplayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.DisplayOrder),
                "Display order cannot be negative.");
        }

        var image =
            await _repository.GetImageByIdAsync(
                request.Id,
                includeTags: true,
                ct)
            ?? throw new NotFoundException(
                nameof(GalleryImage),
                request.Id);

        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.CategoryId);

        /*
         * Existing images are allowed to remain in their current
         * category even if that category has since been deactivated.
         *
         * Moving an image into a different category, however,
         * requires the destination category to be active.
         */
        if (image.CategoryId != request.CategoryId &&
            !category.IsActive)
        {
            throw new InvalidOperationException(
                "An image cannot be moved to an inactive gallery category.");
        }

        image.UpdateMetadata(
            request.Title,
            request.Description,
            request.AltText,
            request.Photographer,
            request.EventDate);

        image.SetCategory(
            request.CategoryId);

        image.SetDisplayOrder(
            request.DisplayOrder);

        await SynchronizeTagsAsync(
            image,
            request.Tags,
            ct);

        GalleryStoredImage? newStoredImage = null;

        string? oldImagePath = null;
        string? oldThumbnailPath = null;

        try
        {
            if (request.NewImageData is not null)
            {
                if (request.NewImageData.Length == 0)
                {
                    throw new ArgumentException(
                        "Replacement image data cannot be empty.",
                        nameof(request.NewImageData));
                }

                if (string.IsNullOrWhiteSpace(
                        request.NewContentType))
                {
                    throw new ArgumentException(
                        "A content type is required when replacing an image.",
                        nameof(request.NewContentType));
                }

                oldImagePath =
                    image.ImagePath;

                oldThumbnailPath =
                    image.ThumbnailPath;

                /*
                 * SaveAsync is intentionally inside this try block.
                 * If processing succeeds but a later operation fails,
                 * the new files can be cleaned up below.
                 */
                newStoredImage =
                    await _imageStorage.SaveAsync(
                        request.NewImageData,
                        request.NewContentType,
                        ct);

                image.ReplaceImage(
                    imagePath:
                        newStoredImage.ImagePath,
                    thumbnailPath:
                        newStoredImage.ThumbnailPath,
                    contentType:
                        newStoredImage.ContentType,
                    fileSizeBytes:
                        newStoredImage.FileSizeBytes,
                    width:
                        newStoredImage.Width,
                    height:
                        newStoredImage.Height,
                    imageHash:
                        newStoredImage.ImageHash);
            }

            await _repository.UpdateImageAsync(
                image,
                ct);

            await _repository.SaveChangesAsync(
                ct);
        }
        catch
        {
            if (newStoredImage is not null)
            {
                try
                {
                    await _imageStorage.DeleteAsync(
                        newStoredImage.ImagePath,
                        newStoredImage.ThumbnailPath,
                        CancellationToken.None);
                }
                catch
                {
                    // Preserve the original exception.
                }
            }

            throw;
        }

        /*
         * Persistence has succeeded. If the physical image was
         * replaced, the previous files are now orphaned and can
         * safely be removed.
         */
        if (newStoredImage is not null)
        {
            try
            {
                await _imageStorage.DeleteAsync(
                    oldImagePath,
                    oldThumbnailPath,
                    CancellationToken.None);
            }
            catch
            {
                // Do not turn a successful database update into
                // a failed request because old-file cleanup failed.
            }
        }

        return await MapToDetailDtoAsync(
            image,
            category.Name,
            ct);
    }

    private async Task SynchronizeTagsAsync(
        GalleryImage image,
        IEnumerable<string>? requestedTags,
        CancellationToken ct)
    {
        var requestedTagNames =
            NormalizeTagNames(
                requestedTags);

        var desiredTagIds =
            new HashSet<Guid>();

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

            desiredTagIds.Add(
                tag.Id);
        }

        var currentTagIds =
            image.Tags
                .Select(tag =>
                    tag.TagId)
                .ToList();

        foreach (var currentTagId in currentTagIds)
        {
            if (!desiredTagIds.Contains(
                    currentTagId))
            {
                image.RemoveTag(
                    currentTagId);
            }
        }

        foreach (var desiredTagId in desiredTagIds)
        {
            image.AddTag(
                desiredTagId);
        }
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

    private Task<GalleryImageDto> MapToDetailDtoAsync(
        GalleryImage image,
        string categoryName,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var tagNames =
            image.Tags
                .Select(tag =>
                    tag.Tag?.Name)
                .Where(name =>
                    !string.IsNullOrWhiteSpace(
                        name))
                .Select(name =>
                    name!)
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        var dto =
            new GalleryImageDto(
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

        return Task.FromResult(
            dto);
    }
}