
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Handles public lookup of celebration invitations
/// using a secure, single-use invitation token.
/// </summary>
public sealed class GetCelebrationInvitationByTokenQueryHandler
    : IRequestHandler<
        GetCelebrationInvitationByTokenQuery,
        CelebrationInvitationDetailsDto?>
{
    private readonly ICelebrationInvitationTokenRepository _tokenRepository;
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initialises the invitation lookup handler.
    /// </summary>
    /// <param name="tokenRepository">
    /// Repository for secure invitation tokens.
    /// </param>
    /// <param name="celebrationRepository">
    /// Repository for celebration records.
    /// </param>
    public GetCelebrationInvitationByTokenQueryHandler(
        ICelebrationInvitationTokenRepository tokenRepository,
        ICelebrationRepository celebrationRepository)
    {
        _tokenRepository = tokenRepository
            ?? throw new ArgumentNullException(nameof(tokenRepository));

        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <inheritdoc />
    public async Task<CelebrationInvitationDetailsDto?> Handle(
        GetCelebrationInvitationByTokenQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return null;
        }

        // Tokens are generated as 64-character hexadecimal strings.
        // Reject malformed tokens before accessing the database.
        if (request.Token.Length != 64 ||
            !request.Token.All(Uri.IsHexDigit))
        {
            return null;
        }

        var tokenHash =
            CelebrationInvitationToken.ComputeTokenHash(
                request.Token);

        var invitation =
            await _tokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (invitation is null)
        {
            return null;
        }

        if (!invitation.IsValid(DateTime.UtcNow))
        {
            return null;
        }

        var celebration =
            await _celebrationRepository.GetByIdAsync(
                invitation.CelebrationId,
                cancellationToken);

        if (celebration is null ||
            celebration.IsDeleted ||
            celebration.Status != "Pending" ||
            !celebration.InvitationSentAt.HasValue ||
            celebration.RespondedAt.HasValue)
        {
            return null;
        }

        return new CelebrationInvitationDetailsDto(
            celebration.Occasion,
            celebration.CelebrationDate,
            invitation.ExpiresAt);
    }
}
