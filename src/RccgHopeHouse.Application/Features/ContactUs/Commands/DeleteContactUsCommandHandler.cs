using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Handler for deleting a contact request.
/// </summary>
public class DeleteContactUsCommandHandler : IRequestHandler<DeleteContactUsCommand, Unit>
{
    private readonly IContactUsRepository _repository;

    public DeleteContactUsCommandHandler(IContactUsRepository repository) => _repository = repository;

    /// <summary>
    /// Loads the request by ID, verifies it exists, and deletes it.
    /// </summary>
    public async Task<Unit> Handle(DeleteContactUsCommand request, CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Core.Entities.ContactUs), request.Id);

        await _repository.DeleteAsync(contact, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}