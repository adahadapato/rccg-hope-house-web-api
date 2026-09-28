using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Commands;

public class DeleteChurchEventCommandHandler
    : IRequestHandler<DeleteChurchEventCommand>
{
    private readonly IChurchEventRepository _repository;

    public DeleteChurchEventCommandHandler(
        IChurchEventRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DeleteChurchEventCommand request, CancellationToken ct)
    {
        var churchEvent =    await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchEvent),  request.Id);

        await _repository.DeleteAsync(churchEvent, ct);
        await _repository.SaveChangesAsync(ct);
    }
}