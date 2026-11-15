using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

public class DeleteDevotionalCommandHandler
    : IRequestHandler<DeleteDevotionalCommand>
{
    private readonly IDevotionalRepository _repository;

    public DeleteDevotionalCommandHandler(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var devotional = await _repository.GetByIdForAdminAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Id);

        await _repository.DeleteAsync(devotional, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}