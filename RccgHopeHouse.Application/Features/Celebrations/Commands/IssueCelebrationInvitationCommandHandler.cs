using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles secure SMS invitations for birthday and wedding anniversary celebrations.
/// </summary>
public sealed class IssueCelebrationInvitationCommandHandler
    : IRequestHandler<IssueCelebrationInvitationCommand, bool>
{
    private readonly ICelebrationRepository _celebrationRepository;
    private readonly ICelebrationInvitationTokenRepository _tokenRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ISmsService _smsService;
    private readonly ILogger<IssueCelebrationInvitationCommandHandler> _logger;

    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(7);

    public IssueCelebrationInvitationCommandHandler(
        ICelebrationRepository celebrationRepository,
        ICelebrationInvitationTokenRepository tokenRepository,
        IMemberRepository memberRepository,
        ISmsService smsService,
        ILogger<IssueCelebrationInvitationCommandHandler> logger)
    {
        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(nameof(celebrationRepository));
        _tokenRepository = tokenRepository
            ?? throw new ArgumentNullException(nameof(tokenRepository));
        _memberRepository = memberRepository
            ?? throw new ArgumentNullException(nameof(memberRepository));
        _smsService = smsService
            ?? throw new ArgumentNullException(nameof(smsService));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(
        IssueCelebrationInvitationCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid celebration ID is required.", nameof(request));
        }

        var celebration = await _celebrationRepository.GetByIdAsync(
            request.CelebrationId, ct);

        if (celebration is null)
        {
            throw new KeyNotFoundException(
                "The selected celebration does not exist.");
        }

        if (celebration.Status != "Pending")
        {
            throw new InvalidOperationException(
                "Only pending celebrations can receive invitations.");
        }

        if (celebration.InvitationSentAt.HasValue)
        {
            throw new InvalidOperationException(
                "An invitation has already been sent for this celebration.");
        }

        var member = await _memberRepository.GetByIdAsync(
            celebration.MemberId, ct);

        if (member is null)
        {
            throw new KeyNotFoundException(
                "The member associated with this celebration does not exist.");
        }

        if (!member.IsActive)
        {
            throw new InvalidOperationException(
                "Invitations cannot be sent to inactive members.");
        }

        if (!member.ConsentToContact)
        {
            throw new InvalidOperationException(
                "The member has not consented to church communications.");
        }

        if (member.PhoneNumber is null)
        {
            throw new InvalidOperationException(
                "The member does not have a recorded telephone number.");
        }

        var phoneNumber = member.PhoneNumber.ToE164();
        var nowUtc = DateTime.UtcNow;

        var activeTokens = await _tokenRepository.GetActiveByCelebrationAsync(
            celebration.Id, nowUtc, ct);

        if (activeTokens.Count > 0)
        {
            throw new InvalidOperationException(
                "An active invitation token already exists for this celebration.");
        }

        var rawToken = CelebrationInvitationToken.GenerateSecureToken();
        var tokenHash = CelebrationInvitationToken.ComputeTokenHash(rawToken);

        var invitation = CelebrationInvitationToken.Create(
            celebration.Id,
            tokenHash,
            nowUtc,
            nowUtc.Add(InvitationLifetime));

        var invitationUrl = _smsService.BuildCelebrationInvitationUrl(rawToken);

        var occasion = celebration.Occasion == "Birthday"
            ? "birthday"
            : "wedding anniversary";

        var message =
            $"Dear {member.FirstName}, your {occasion} is approaching! " +
            "RCCG Hope House would love to celebrate with you. " +
            $"Please confirm your preference here: {invitationUrl}";


        // Persist first so that an accepted SMS points to a valid token.
        await _tokenRepository.AddAsync(invitation, ct);
        await _tokenRepository.SaveChangesAsync(ct);

        SmsSendResult result;

        try
        {
            result = await _smsService.SendAsync(phoneNumber, message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected SMS submission exception for celebration {CelebrationId}. " +
                "No invitation URL or recipient number is logged.",
                celebration.Id);

            // Preserve the original exception even if best-effort revocation fails.
            try
            {
                invitation.Revoke(DateTime.UtcNow);
                await _tokenRepository.UpdateAsync(invitation, CancellationToken.None);
                await _tokenRepository.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception revokeEx)
            {
                _logger.LogError(
                    revokeEx,
                    "Unable to revoke invitation token following SMS exception for celebration {CelebrationId}.",
                    celebration.Id);
            }

            throw;
        }

        if (!result.IsSuccessful)
        {
            _logger.LogError(
                "SMS provider rejected celebration invitation {CelebrationId}. " +
                "Provider status: {ProviderStatus}; provider error: {ProviderError}; message SID: {MessageSid}.",
                celebration.Id,
                result.Status,
                result.ErrorMessage,
                result.MessageId);

            invitation.Revoke(DateTime.UtcNow);
            await _tokenRepository.UpdateAsync(invitation, ct);
            await _tokenRepository.SaveChangesAsync(ct);

            throw new InvalidOperationException(
                "The SMS provider did not accept the celebration invitation. " +
                "Check the server logs for the provider rejection reason.");
        }

        _logger.LogInformation(
            "SMS invitation accepted for celebration {CelebrationId}. " +
            "Provider status: {ProviderStatus}; message SID: {MessageSid}.",
            celebration.Id,
            result.Status,
            result.MessageId);

        celebration.MarkInvitationSent(DateTime.UtcNow);
        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return true;
    }
}
