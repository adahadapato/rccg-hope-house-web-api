using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

/// <summary>
/// Handles creation of a gallery category.
/// </summary>
public class CreateGalleryCategoryCommandHandler
    : IRequestHandler<CreateGalleryCategoryCommand, GalleryCategoryDto>
{
    private readonly IGalleryCategoryRepository _repository;

    public CreateGalleryCategoryCommandHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<GalleryCategoryDto> Handle(
        CreateGalleryCategoryCommand request,
        CancellationToken ct)
    {
        var category = GalleryCategory.Create(
            name: request.Name,
            displayOrder: request.DisplayOrder,
            description: request.Description);

        await _repository.AddAsync(category, ct);
        await _repository.SaveChangesAsync(ct);

        return GalleryCategoryDto.FromEntity(category);
    }
}