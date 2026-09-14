using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Handler for marking a contact request as read.
/// Uses domain method to enforce business rules.
/// </summary>
public class MarkContactUsAsReadCommandHandler : IRequestHandler<MarkContactUsAsReadCommand, Unit>
{
    private readonly IContactUsRepository _repository;

    public MarkContactUsAsReadCommandHandler(IContactUsRepository repository) => _repository = repository;

    public async Task<Unit> Handle(MarkContactUsAsReadCommand request, CancellationToken ct)
    {
        var contact = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ContactUs), request.Id);

        // Domain method ensures state transitions are tracked
        contact.MarkAsRead();

        await _repository.UpdateAsync(contact, ct);
        await _repository.SaveChangesAsync(ct);
        return Unit.Value;
    }
}