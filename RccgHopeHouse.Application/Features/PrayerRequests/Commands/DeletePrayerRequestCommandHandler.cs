using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Handles the permanent deletion of a prayer request.
/// </summary>
public sealed class DeletePrayerRequestCommandHandler
    : IRequestHandler<
        DeletePrayerRequestCommand,
        Unit>
{
    private readonly IPrayerRequestRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DeletePrayerRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and delete prayer requests.
    /// </param>
    public DeletePrayerRequestCommandHandler(
        IPrayerRequestRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves the specified prayer request and permanently
    /// deletes it from the data store.
    /// </summary>
    /// <param name="request">
    /// The command containing the identifier of the prayer
    /// request to delete.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// A MediatR unit result when the prayer request has been deleted.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified prayer request does not exist.
    /// </exception>
    public async Task<Unit> Handle(
        DeletePrayerRequestCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var prayer =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(PrayerRequest),
                request.Id);

        await _repository.DeleteAsync(
            prayer,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        return Unit.Value;
    }
}