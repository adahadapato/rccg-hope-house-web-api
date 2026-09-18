using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

public class DeleteServiceBroadcastCommandHandler : IRequestHandler<DeleteServiceBroadcastCommand, Unit>
{
    private readonly IServiceBroadcastRepository _repository;

    public DeleteServiceBroadcastCommandHandler(IServiceBroadcastRepository repository) => _repository = repository;

    public async Task<Unit> Handle(DeleteServiceBroadcastCommand request, CancellationToken ct)
    {
        var broadcast = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ServiceBroadcast), request.Id);

        await _repository.DeleteAsync(broadcast, ct);
        await _repository.SaveChangesAsync(ct);
        return Unit.Value;
    }
}