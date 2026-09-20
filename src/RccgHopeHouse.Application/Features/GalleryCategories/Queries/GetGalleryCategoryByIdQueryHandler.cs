using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Queries;

public class GetGalleryCategoryByIdQueryHandler
    : IRequestHandler<GetGalleryCategoryByIdQuery, GalleryCategoryDto>
{
    private readonly IGalleryCategoryRepository _repository;

    public GetGalleryCategoryByIdQueryHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<GalleryCategoryDto> Handle(
        GetGalleryCategoryByIdQuery request,
        CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.Id);

        return GalleryCategoryDto.FromEntity(category);
    }
}