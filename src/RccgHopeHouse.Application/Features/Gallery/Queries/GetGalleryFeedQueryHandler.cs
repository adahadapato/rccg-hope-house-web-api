using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public class GetGalleryFeedQueryHandler : IRequestHandler<GetGalleryFeedQuery, IReadOnlyList<GalleryImageFeedDto>>
{
    private readonly IGalleryRepository _repository;

    public GetGalleryFeedQueryHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<GalleryImageFeedDto>> Handle(GetGalleryFeedQuery request, CancellationToken ct)
    {
        var images = await _repository.GetImagesAsync(
            categoryId: request.CategoryId,
            tagId: request.TagId,
            isFeatured: request.IsFeatured,
            publicOnly: true,
            skip: request.Skip,
            take: request.Take,
            ct: ct);

        var categories = await _repository.GetAllCategoriesAsync(true, ct);
        var tags = await _repository.GetAllTagsAsync(ct);

        return images.Select(img => new GalleryImageFeedDto(
            img.Id, img.Title, img.ThumbnailData, img.ContentType, img.AltText, img.CategoryId,
            categories.FirstOrDefault(c => c.Id == img.CategoryId)?.Name ?? "Uncategorized",
            img.Tags.Select(t => tags.First(tg => tg.Id == t.TagId).Name).ToList(),
            img.IsFeatured, img.EventDate, img.DisplayOrder)).ToList();
    }
}