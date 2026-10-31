
using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Processes a member's response to a secure birthday or
/// wedding anniversary invitation.
/// </summary>
/// <remarks>
/// Validates the invitation token, retrieves the associated
/// celebration, records the member's publication decision,
/// and consumes the single-use invitation token.
///
/// Both entity changes are saved through the existing
/// shared EF Core database context.
/// </remarks>
public sealed class RespondToCelebrationInvitationCommandHandler
    : IRequestHandler<RespondToCelebrationInvitationCommand, bool>
{
    private readonly ICelebrationInvitationTokenRepository _tokenRepository;
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initialises the invitation response handler.
    /// </summary>
    /// <param name="tokenRepository">
    /// Repository for celebration invitation tokens.
    /// </param>
    /// <param name="celebrationRepository">
    /// Repository for celebration records.
    /// </param>
    public RespondToCelebrationInvitationCommandHandler(
        ICelebrationInvitationTokenRepository tokenRepository,
        ICelebrationRepository celebrationRepository)
    {
        _tokenRepository = tokenRepository
            ?? throw new ArgumentNullException(nameof(tokenRepository));

        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(
        RespondToCelebrationInvitationCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Defence-in-depth: validate the token even if
        // the FluentValidation pipeline is not invoked.
        if (string.IsNullOrWhiteSpace(request.Token) ||
            request.Token.Length != 64 ||
            !request.Token.All(Uri.IsHexDigit))
        {
            throw new InvalidOperationException(
                "The invitation is invalid or no longer available.");
        }

        // Only the SHA-256 hash is used for database lookup.
        var tokenHash =
            CelebrationInvitationToken.ComputeTokenHash(
                request.Token);

        var invitation =
            await _tokenRepository.GetByTokenHashAsync(
                tokenHash,
                ct);

        if (invitation is null)
        {
            throw new InvalidOperationException(
                "The invitation is invalid or no longer available.");
        }

        var nowUtc = DateTime.UtcNow;

        if (!invitation.IsValid(nowUtc))
        {
            throw new InvalidOperationException(
                "The invitation is invalid or no longer available.");
        }

        var celebration =
            await _celebrationRepository.GetByIdAsync(
                invitation.CelebrationId,
                ct);

        if (celebration is null || celebration.IsDeleted)
        {
            throw new InvalidOperationException(
                "The invitation is invalid or no longer available.");
        }

        // Ensure the celebration is still awaiting a response.
        // The domain methods also enforce these rules.
        if (celebration.Status != "Pending" ||
            !celebration.InvitationSentAt.HasValue ||
            celebration.RespondedAt.HasValue)
        {
            throw new InvalidOperationException(
                "The invitation is invalid or no longer available.");
        }

        // Record the member's publication decision.
        if (request.Approved)
        {
            celebration.ApprovePublication(nowUtc);
        }
        else
        {
            celebration.Decline(nowUtc);
        }

        // Consume the token using the same timestamp.
        invitation.MarkAsUsed(nowUtc);

        // Both repositories use the same scoped
        // ApplicationDbContext.
        await _celebrationRepository.UpdateAsync(
            celebration,
            ct);

        await _tokenRepository.UpdateAsync(
            invitation,
            ct);

        // A single SaveChangesAsync persists both changes
        // atomically under EF Core's normal transaction behaviour.
        await _celebrationRepository.SaveChangesAsync(ct);

        return true;
    }
}
