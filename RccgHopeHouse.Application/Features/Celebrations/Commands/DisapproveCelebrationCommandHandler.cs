
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles a member's decision to decline celebration publication.
/// </summary>
/// <remarks>
/// A member may decline publication only after receiving an invitation
/// and before submitting another response.
///
/// Declining publication changes the celebration status to Declined,
/// preventing it from progressing to administrator approval or publication.
///
/// The caller must verify member identity or validate a secure invitation
/// token before dispatching this command.
/// </remarks>
public sealed class DisapproveCelebrationCommandHandler
    : IRequestHandler<DisapproveCelebrationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes the celebration disapproval handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve and update celebrations.</param>
    public DisapproveCelebrationCommandHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Records the member's decision to decline publication.
    /// </summary>
    /// <param name="request">The command identifying the celebration.</param>
    /// <param name="ct">Cancellation token for repository operations.</param>
    /// <returns>The updated celebration DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when the celebration identifier is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the celebration does not exist.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the celebration is not awaiting a member response.
    /// </exception>
    public async Task<CelebrationDto> Handle(DisapproveCelebrationCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
            throw new ArgumentException("A valid celebration ID is required.", nameof(request.CelebrationId));

        // Retrieve the celebration and its associated entities.
        var celebration = await _celebrationRepository.GetByIdAsync(request.CelebrationId, ct);

        if (celebration is null)
            throw new KeyNotFoundException("The selected celebration does not exist.");

        // Verify that the member is eligible to respond.
        if (celebration.Status != "Pending" || !celebration.InvitationSentAt.HasValue || celebration.RespondedAt.HasValue)
            throw new InvalidOperationException("This celebration is not awaiting a member response.");

        // Record the member's refusal using a UTC response timestamp.
        celebration.Decline(DateTime.UtcNow);

        // Persist the decision.
        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return CelebrationDto.FromEntity(celebration);
    }
}
