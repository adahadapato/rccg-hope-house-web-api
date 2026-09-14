using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

//public class GetGalleryCategoriesQueryHandler : IRequestHandler<GetGalleryCategoriesQuery, IReadOnlyList<GalleryCategoryDto>>
//{
//    private readonly IGalleryRepository _repository;

//    public GetGalleryCategoriesQueryHandler(IGalleryRepository repository) => _repository = repository;

//    public async Task<IReadOnlyList<GalleryCategoryDto>> Handle(GetGalleryCategoriesQuery request, CancellationToken ct)
//    {
//        var categories = await _repository.GetAllCategoriesAsync(includeImageCount: true, ct);
//        if (!request.IncludeInactive) categories = categories.Where(c => c.IsActive).ToList();

//        return categories.Select(c => new GalleryCategoryDto(c.Id, c.Name, c.Description, c.DisplayOrder, c.Images.Count)).ToList();
//    }
//}


// Application/Features/Gallery/Queries/GetGalleryCategoriesQueryHandler.cs
public class GetGalleryCategoriesQueryHandler : IRequestHandler<GetGalleryCategoriesQuery, IReadOnlyList<GalleryCategoryDto>>
{
    private readonly IGalleryRepository _repository;

    public GetGalleryCategoriesQueryHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<GalleryCategoryDto>> Handle(GetGalleryCategoriesQuery request, CancellationToken ct)
    {
        var categories = await _repository.GetAllCategoriesAsync(true, ct);

        // Simple in-memory mapping; negligible overhead for small lists
        return categories.Select(c => new GalleryCategoryDto(
            Id: c.Id,
            Name: c.Name,
            Description: c.Description,
            DisplayOrder: c.DisplayOrder,
            //IsActive: c.IsActive,
            ImageCount: 0 // Fetch separately if needed via another query
        )).ToList();
    }
}