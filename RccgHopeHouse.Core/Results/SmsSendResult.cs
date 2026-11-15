namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Represents the result of an SMS submission attempt.
///
/// A successful submission means the provider accepted the request.
/// It does not guarantee delivery to the recipient's handset.
/// </summary>
/// <param name="IsSuccessful">
/// Indicates whether the SMS provider accepted the submission.
/// </param>
/// <param name="MessageId">
/// The provider's unique message identifier, if available.
/// </param>
/// <param name="Status">
/// The message status reported by the provider.
/// </param>
/// <param name="ErrorMessage">
/// An error description when the submission fails.
/// </param>
public sealed record SmsSendResult(
    bool IsSuccessful,
    string? MessageId,
    string? Status,
    string? ErrorMessage);
