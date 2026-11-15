using MediatR;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Commands;

public class DeactivateGalleryCategoryCommandHandler
    : IRequestHandler<DeactivateGalleryCategoryCommand, GalleryCategoryDto>
{
    private readonly IGalleryCategoryRepository _repository;

    public DeactivateGalleryCategoryCommandHandler(
        IGalleryCategoryRepository repository) =>
        _repository = repository;

    public async Task<GalleryCategoryDto> Handle(
        DeactivateGalleryCategoryCommand request,
        CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(
                nameof(GalleryCategory),
                request.Id);

        category.Deactivate();

        await _repository.UpdateAsync(category, ct);
        await _repository.SaveChangesAsync(ct);

        return GalleryCategoryDto.FromEntity(category);
    }
}