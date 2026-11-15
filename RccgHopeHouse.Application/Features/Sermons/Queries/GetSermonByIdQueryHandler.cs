using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Sermons.Queries;

public class GetSermonByIdQueryHandler : IRequestHandler<GetSermonByIdQuery, SermonDto>
{
    private readonly ISermonRepository _repository;
    public GetSermonByIdQueryHandler(ISermonRepository repository) => _repository = repository;

    public async Task<SermonDto> Handle(GetSermonByIdQuery request, CancellationToken ct)
    {
        var sermon = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Sermon), request.Id);

        // ✅ Fixed: Pass all 8 required parameters in exact record order
        return new SermonDto(
            sermon.Id,
            sermon.Title,
            sermon.Speaker,
            sermon.ServiceDate,
            sermon.VideoUrl,
            sermon.Description,
            sermon.IsPublished, // ← Added
            sermon.CreatedAt);  // ← Added
    }
}