
using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles administrative requests to soft-delete
/// existing celebration records.
/// </summary>
/// <remarks>
/// Retrieves the celebration, verifies that it has not
/// been published, and delegates logical deletion to
/// the celebration repository.
///
/// Soft deletion preserves the underlying database record
/// while excluding it from normal repository queries.
///
/// This operation does not remove gallery images or
/// physically delete uploaded files.
/// </remarks>
public sealed class DeleteCelebrationCommandHandler
    : IRequestHandler<DeleteCelebrationCommand, bool>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes the celebration deletion handler.
    /// </summary>
    /// <param name="celebrationRepository">
    /// Repository responsible for celebration persistence
    /// and logical deletion.
    /// </param>
    public DeleteCelebrationCommandHandler(
        ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(
                nameof(celebrationRepository));
    }

    /// <summary>
    /// Validates and soft-deletes an existing celebration.
    /// </summary>
    /// <param name="request">
    /// The command containing the celebration identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token for asynchronous operations.
    /// </param>
    /// <returns>
    /// True when the celebration has been successfully
    /// marked for deletion and the changes persisted.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the celebration identifier is empty.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the celebration does not exist or
    /// has already been soft-deleted.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the celebration has already been
    /// published or withdrawn after publication.
    /// </exception>
    public async Task<bool> Handle(
        DeleteCelebrationCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate the celebration identifier.
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid celebration ID is required.",
                nameof(request.Id));
        }

        // Retrieve the existing celebration.
        var celebration =
            await _celebrationRepository.GetByIdAsync(
                request.Id,
                ct);

        if (celebration is null)
        {
            throw new KeyNotFoundException(
                "The selected celebration does not exist.");
        }

        // Preserve the history of any celebration
        // that has previously been published.
        if (celebration.PublishedAt.HasValue ||
            celebration.Status is "Published" or "Withdrawn")
        {
            throw new InvalidOperationException(
                "A published or withdrawn celebration cannot be deleted. Use the withdrawal workflow where applicable.");
        }

        // Do not delete a celebration that has already
        // received administrator approval.
        if (celebration.ApprovedByAdminAt.HasValue)
        {
            throw new InvalidOperationException(
                "An administrator-approved celebration cannot be deleted.");
        }

        // Mark the celebration as logically deleted.
        await _celebrationRepository.SoftDeleteAsync(
            celebration,
            ct);

        // Persist the deletion.
        await _celebrationRepository.SaveChangesAsync(ct);

        return true;
    }
}
