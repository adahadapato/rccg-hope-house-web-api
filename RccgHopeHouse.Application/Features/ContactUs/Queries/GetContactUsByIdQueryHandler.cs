using MediatR;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Queries;

/// <summary>
/// Handles retrieval of an individual Contact Us submission
/// for administrative review.
/// </summary>
public sealed class GetContactUsByIdQueryHandler
    : IRequestHandler<
        GetContactUsByIdQuery,
        ContactUsDto>
{
    private readonly IContactUsRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetContactUsByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve Contact Us submissions.
    /// </param>
    public GetContactUsByIdQueryHandler(
        IContactUsRepository repository)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    /// <summary>
    /// Retrieves the requested Contact Us submission.
    /// </summary>
    /// <param name="request">
    /// Query containing the contact submission identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The matching Contact Us DTO.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the requested submission does not exist.
    /// </exception>
    public async Task<ContactUsDto> Handle(
        GetContactUsByIdQuery request,
        CancellationToken ct)
    {
        var contact =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ContactUs),
                request.Id);

        return ContactUsDto.FromEntity(
            contact);
    }
}