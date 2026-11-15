using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Queries;

public class GetActiveGalleryCategoriesQueryHandler
    : IRequestHandler<
        GetActiveGalleryCategoriesQuery,
        IReadOnlyList<GalleryCategoryDto>>
{
    private readonly IGalleryCategoryRepository _repository;

    public GetActiveGalleryCategoriesQueryHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<GalleryCategoryDto>> Handle(
        GetActiveGalleryCategoriesQuery request,
        CancellationToken ct)
    {
        var categories = await _repository.GetActiveAsync(ct);

        return categories
            .Select(GalleryCategoryDto.FromEntity)
            .ToList();
    }
}