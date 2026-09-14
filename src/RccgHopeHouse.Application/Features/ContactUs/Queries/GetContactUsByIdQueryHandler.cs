using MediatR;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Queries;

/// <summary>
/// Handler for fetching a single contact request.
/// Throws NotFoundException if the ID doesn't exist.
/// </summary>
public class GetContactUsByIdQueryHandler : IRequestHandler<GetContactUsByIdQuery, ContactUsDto>
{
    private readonly IContactUsRepository _repository;

    public GetContactUsByIdQueryHandler(IContactUsRepository repository) => _repository = repository;

    public async Task<ContactUsDto> Handle(GetContactUsByIdQuery request, CancellationToken ct)
    {
        var contact = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ContactUs), request.Id);

        return new ContactUsDto(
            contact.Id, contact.FirstName, contact.LastName, contact.Email,
            contact.PhoneNumber, contact.Reason, contact.Message, contact.IsRead, contact.CreatedAt);
    }
}