using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

/// <summary>
/// Handles updates to a gallery category.
/// </summary>
public class UpdateGalleryCategoryCommandHandler
    : IRequestHandler<UpdateGalleryCategoryCommand, GalleryCategoryDto>
{
    private readonly IGalleryCategoryRepository _repository;

    public UpdateGalleryCategoryCommandHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<GalleryCategoryDto> Handle(
        UpdateGalleryCategoryCommand request,
        CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.Id);

        category.Update(
            name: request.Name,
            description: request.Description,
            displayOrder: request.DisplayOrder);

        await _repository.UpdateAsync(category, ct);
        await _repository.SaveChangesAsync(ct);

        return GalleryCategoryDto.FromEntity(category);
    }
}