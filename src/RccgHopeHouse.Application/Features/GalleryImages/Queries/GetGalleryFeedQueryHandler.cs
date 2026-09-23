using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public class GetGalleryFeedQueryHandler
    : IRequestHandler<
        GetGalleryFeedQuery,
        IReadOnlyList<GalleryImageFeedDto>>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;

    public GetGalleryFeedQueryHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<GalleryImageFeedDto>> Handle(
        GetGalleryFeedQuery request,
        CancellationToken ct)
    {
        var images =
            await _repository.GetImagesAsync(
                categoryId: request.CategoryId,
                tagId: request.TagId,
                isFeatured: request.IsFeatured,
                publicOnly: true,
                skip: request.Skip,
                take: request.Take,
                ct: ct);

        var categories =
            await _categoryRepository.GetActiveAsync(
                ct);

        var tags =
            await _repository.GetAllTagsAsync(
                ct);

        return images
            .Select(image =>
                new GalleryImageFeedDto(
                    image.Id,
                    image.Title,
                    image.ThumbnailPath,
                    image.ContentType,
                    image.AltText,
                    image.CategoryId,
                    categories
                        .FirstOrDefault(
                            category =>
                                category.Id ==
                                image.CategoryId)
                        ?.Name
                        ?? "Uncategorized",
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
                        .ToList(),
                    image.Width,
                    image.Height,
                    image.IsFeatured,
                    image.EventDate,
                    image.DisplayOrder))
            .ToList();
    }
}