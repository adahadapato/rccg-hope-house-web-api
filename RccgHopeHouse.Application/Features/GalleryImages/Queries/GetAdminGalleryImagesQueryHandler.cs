using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

/// <summary>
/// Handles administrative gallery image retrieval.
///
/// Administrative retrieval differs from the public feed because
/// private images must remain visible to authorised administrators
/// so that they can be edited, re-published, featured or deleted.
/// </summary>
public class GetAdminGalleryImagesQueryHandler
    : IRequestHandler<
        GetAdminGalleryImagesQuery,
        IReadOnlyList<GalleryImageDto>>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;

    public GetAdminGalleryImagesQueryHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<GalleryImageDto>> Handle(
        GetAdminGalleryImagesQuery request,
        CancellationToken ct)
    {
        var take = Math.Clamp(
            request.Take,
            1,
            200);

        var skip = Math.Max(
            request.Skip,
            0);

        var images =
            await _repository.GetImagesAsync(
                categoryId: request.CategoryId,
                tagId: request.TagId,
                isFeatured: request.IsFeatured,
                publicOnly: false,
                skip: skip,
                take: take,
                ct: ct);

        var categories =
            await _categoryRepository.GetAllAsync(
                ct);

        var categoryNames =
            categories.ToDictionary(
                category => category.Id,
                category => category.Name);

        return images
            .Select(image =>
            {
                var categoryName =
                    categoryNames.TryGetValue(
                        image.CategoryId,
                        out var name)
                        ? name
                        : "Uncategorized";

                var tagNames =
                    image.Tags
                        .Where(imageTag =>
                            imageTag.Tag is not null)
                        .Select(imageTag =>
                            imageTag.Tag!.Name)
                        .Where(tagName =>
                            !string.IsNullOrWhiteSpace(
                                tagName))
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase)
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
            })
            .ToList();
    }
}