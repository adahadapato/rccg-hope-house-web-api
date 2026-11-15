
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles administrator approval of member celebrations.
/// </summary>
/// <remarks>
/// Ensures the member has approved publication and that the celebration
/// has not already been approved, published, declined or withdrawn.
/// </remarks>
public sealed class ApproveCelebrationByAdminCommandHandler : IRequestHandler<ApproveCelebrationByAdminCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes a new instance of the administrator approval handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve and update celebrations.</param>
    public ApproveCelebrationByAdminCommandHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Approves a member's celebration for publication.
    /// </summary>
    /// <param name="request">The administrator approval command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated celebration DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when the celebration identifier is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the celebration cannot be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the celebration is not eligible for approval.</exception>
    public async Task<CelebrationDto> Handle(ApproveCelebrationByAdminCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
            throw new ArgumentException("A valid celebration ID is required.", nameof(request.CelebrationId));

        var celebration = await _celebrationRepository.GetByIdAsync(request.CelebrationId, ct);

        if (celebration is null)
            throw new KeyNotFoundException("The selected celebration does not exist.");

        if (celebration.Status != "ApprovedByMember" || !celebration.RespondedAt.HasValue)
            throw new InvalidOperationException("The celebration must have member approval before administrator approval.");

        if (celebration.PhotoUrl is not null && !celebration.PhotoPublicationConsent)
            throw new InvalidOperationException("The selected photograph does not have publication consent.");

        celebration.ApproveByAdmin(DateTime.UtcNow);

        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return CelebrationDto.FromEntity(celebration);
    }
}
