using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Handler for deleting a church service.
/// </summary>
public class DeleteChurchServiceCommandHandler : IRequestHandler<DeleteChurchServiceCommand, Unit>
{
    private readonly IChurchServiceRepository _repository;

    public DeleteChurchServiceCommandHandler(IChurchServiceRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeleteChurchServiceCommand request, CancellationToken ct)
    {
        var service = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchService), request.Id);

        await _repository.DeleteAsync(service, ct);
        await _repository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}