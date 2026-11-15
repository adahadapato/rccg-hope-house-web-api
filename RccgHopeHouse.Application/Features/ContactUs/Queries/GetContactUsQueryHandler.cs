using MediatR;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Queries;

/// <summary>
/// Handles retrieval of Contact Us submissions for the
/// administrative contact inbox.
/// </summary>
public sealed class GetContactUsQueryHandler
    : IRequestHandler<
        GetContactUsQuery,
        IReadOnlyList<ContactUsDto>>
{
    private readonly IContactUsRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetContactUsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve Contact Us submissions.
    /// </param>
    public GetContactUsQueryHandler(
        IContactUsRepository repository)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));
    }

    /// <summary>
    /// Retrieves Contact Us submissions according to the requested
    /// unread filter and pagination values.
    /// </summary>
    /// <param name="request">
    /// Query containing filtering and pagination options.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Contact submissions mapped to DTOs.
    /// </returns>
    public async Task<IReadOnlyList<ContactUsDto>> Handle(
        GetContactUsQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        var skip =
            Math.Max(
                request.Skip,
                0);

        var take =
            Math.Clamp(
                request.Take,
                1,
                100);

        IReadOnlyList<Core.Entities.ContactUs> contacts =
            request.UnreadOnly
                ? await _repository.GetUnreadAsync(
                    skip,
                    take,
                    ct)
                : await _repository.GetAllAsync(
                    skip,
                    take,
                    ct);

        return contacts
            .Select(
                ContactUsDto.FromEntity)
            .ToList();
    }
}