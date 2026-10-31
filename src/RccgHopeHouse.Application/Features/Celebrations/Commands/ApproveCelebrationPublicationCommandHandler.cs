
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles a member's approval of celebration publication.
/// </summary>
/// <remarks>
/// Records the member's consent to publish celebration details.
///
/// Approval is permitted only when an invitation has been sent
/// and the celebration is awaiting its first member response.
///
/// Administrator approval and actual publication remain separate operations.
///
/// The calling application must verify the responding member's identity
/// or validate a secure invitation token before dispatching this command.
/// </remarks>
public sealed class ApproveCelebrationPublicationCommandHandler
    : IRequestHandler<ApproveCelebrationPublicationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes the member publication approval handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve and update celebrations.</param>
    public ApproveCelebrationPublicationCommandHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Records the member's approval and persists the updated celebration.
    /// </summary>
    /// <param name="request">The command identifying the celebration.</param>
    /// <param name="ct">Cancellation token for repository operations.</param>
    /// <returns>The updated celebration DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when the celebration identifier is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the celebration does not exist.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the celebration is not awaiting a member response.
    /// </exception>
    public async Task<CelebrationDto> Handle(ApproveCelebrationPublicationCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
            throw new ArgumentException("A valid celebration ID is required.", nameof(request.CelebrationId));

        // Retrieve the celebration and its associated entities.
        var celebration = await _celebrationRepository.GetByIdAsync(request.CelebrationId, ct);

        if (celebration is null)
            throw new KeyNotFoundException("The selected celebration does not exist.");

        // Verify that the invitation has been sent and no response exists.
        if (celebration.Status != "Pending" || !celebration.InvitationSentAt.HasValue || celebration.RespondedAt.HasValue)
            throw new InvalidOperationException("This celebration is not awaiting a member response.");

        // Apply the domain transition using a UTC response timestamp.
        celebration.ApprovePublication(DateTime.UtcNow);

        // Persist the member's consent.
        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return CelebrationDto.FromEntity(celebration);
    }
}
