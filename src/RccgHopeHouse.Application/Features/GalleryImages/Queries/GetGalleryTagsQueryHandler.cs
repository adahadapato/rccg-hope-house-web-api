using MediatR;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Gallery.Queries;

public class GetGalleryTagsQueryHandler : IRequestHandler<GetGalleryTagsQuery, IReadOnlyList<GalleryTagDto>>
{
    private readonly IGalleryRepository _repository;

    public GetGalleryTagsQueryHandler(IGalleryRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<GalleryTagDto>> Handle(GetGalleryTagsQuery request, CancellationToken ct)
    {
        var tags = await _repository.GetAllTagsAsync(ct);
        return tags.Select(t => new GalleryTagDto(t.Id, t.Name, t.Description, t.Images.Count)).ToList();
    }
}