using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Queries;

public class GetAllGalleryCategoriesQueryHandler
    : IRequestHandler<
        GetAllGalleryCategoriesQuery,
        IReadOnlyList<GalleryCategoryDto>>
{
    private readonly IGalleryCategoryRepository _repository;

    public GetAllGalleryCategoriesQueryHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<GalleryCategoryDto>> Handle(
        GetAllGalleryCategoriesQuery request,
        CancellationToken ct)
    {
        var categories = await _repository.GetAllAsync(ct);

        return categories
            .Select(GalleryCategoryDto.FromEntity)
            .ToList();
    }
}