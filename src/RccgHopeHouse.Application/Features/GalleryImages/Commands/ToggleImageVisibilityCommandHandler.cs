using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Commands;

public class ToggleImageVisibilityCommandHandler
    : IRequestHandler<ToggleImageVisibilityCommand, GalleryImageDto>
{
    private readonly IGalleryRepository _repository;
    private readonly IGalleryCategoryRepository _categoryRepository;

    public ToggleImageVisibilityCommandHandler(
        IGalleryRepository repository,
        IGalleryCategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<GalleryImageDto> Handle(
        ToggleImageVisibilityCommand request,
        CancellationToken ct)
    {
        var image = await _repository.GetImageByIdAsync(
            request.Id,
            includeTags: true,
            ct)
            ?? throw new NotFoundException(
                nameof(GalleryImage),
                request.Id);

        image.TogglePublic();

        await _repository.UpdateImageAsync(
            image,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        var category =
            await _categoryRepository.GetByIdAsync(
                image.CategoryId,
                ct);

        var tagNames = image.Tags
            .Select(tag => tag.Tag?.Name)
            .Where(name =>
                !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
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
            category?.Name ?? "Uncategorized",
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
    }
}