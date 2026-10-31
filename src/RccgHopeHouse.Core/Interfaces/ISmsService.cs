
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines provider-independent SMS messaging operations.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS message to the specified recipient.
    /// </summary>
    /// <param name="toPhoneNumber">
    /// Recipient telephone number in E.164 format.
    /// </param>
    /// <param name="message">SMS message content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The SMS submission result.</returns>
    Task<SmsSendResult> SendAsync(
        string toPhoneNumber,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds the public website URL for responding
    /// to a celebration invitation.
    /// </summary>
    /// <param name="token">The secure invitation token.</param>
    /// <returns>The public invitation response URL.</returns>
    string BuildCelebrationInvitationUrl(string token);
}
