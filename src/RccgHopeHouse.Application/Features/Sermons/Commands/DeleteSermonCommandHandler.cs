using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Sermons.Commands;

public class DeleteSermonCommandHandler : IRequestHandler<DeleteSermonCommand, Unit>
{
    private readonly ISermonRepository _repository;
    public DeleteSermonCommandHandler(ISermonRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeleteSermonCommand request, CancellationToken ct)
    {
        var sermon = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.Sermon), request.Id);

        await _repository.DeleteAsync(sermon, ct);
        await _repository.SaveChangesAsync(ct);
        return Unit.Value;
    }
}