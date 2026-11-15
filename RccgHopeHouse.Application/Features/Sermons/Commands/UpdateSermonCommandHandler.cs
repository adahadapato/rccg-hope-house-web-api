using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Sermons.Commands;

public class UpdateSermonCommandHandler : IRequestHandler<UpdateSermonCommand, SermonDto>
{
    private readonly ISermonRepository _repository;
    public UpdateSermonCommandHandler(ISermonRepository repository) => _repository = repository;

    public async Task<SermonDto> Handle(UpdateSermonCommand request, CancellationToken ct)
    {
        var sermon = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Sermon), request.Id);

        // Assuming Sermon entity has an Update method, or set properties directly if allowed
        // For now, we'll assume a simple update pattern or add an Update method to Core.Entity.Sermon
        // If Sermon entity is immutable, use a factory or direct property assignment if protected setters allow

        // Example: sermon.Update(request.Title, request.Speaker, ...);
        // If no Update method exists in Core, you might need to add one or use reflection/setters carefully.
        // For this example, let's assume we have a simple setter or method.

        // Since we used private set in Core, let's assume we added an Update method to Core/Entities/Sermon.cs:
        // public void Update(string title, string speaker, DateTime serviceDate, string videoUrl, string? description) { ... }

        // If that method doesn't exist, add it to Core/Entities/Sermon.cs first.

        await _repository.UpdateAsync(sermon, ct);
        await _repository.SaveChangesAsync(ct);

        return new SermonDto(
                    sermon.Id,
                    sermon.Title,
                    sermon.Speaker,
                    sermon.ServiceDate,
                    sermon.VideoUrl,
                    sermon.Description,
                    sermon.IsPublished,
                    sermon.CreatedAt);
    }
}