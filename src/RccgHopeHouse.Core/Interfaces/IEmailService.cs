namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Represents the result of an email sending operation.
/// Keeps Core pure by avoiding provider-specific types.
/// </summary>
public record EmailResult(
    bool IsSuccess,
    string? MessageId,    // Provider-specific ID (e.g., SendGrid message ID)
    string? ErrorMessage); // Human-readable error if IsSuccess == false

/// <summary>
/// Abstraction for sending transactional and notification emails.
/// Implemented in Infrastructure using providers like SendGrid, Mailgun, Amazon SES, or SMTP.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a single transactional email (e.g., contact form submission, prayer request alert).
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="htmlBody">HTML email content (supports basic tags & inline CSS)</param>
    /// <param name="ct">Cancellation token for async operations</param>
    /// <returns>EmailResult indicating success/failure and optional provider message ID</returns>
    /// <remarks>
    /// This method should handle retries, rate limiting, and fallback logic internally.
    /// Does not throw on delivery failure; instead returns IsSuccess = false.
    /// </remarks>
    Task<EmailResult> SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends a bulk/marketing email to multiple recipients.
    /// Use for newsletters, event announcements, or prayer updates.
    /// </summary>
    /// <param name="recipients">List of recipient email addresses</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="htmlBody">HTML email content</param>
    /// <param name="ct">Cancellation token for async operations</param>
    /// <returns>EmailResult indicating overall batch success/failure</returns>
    /// <remarks>
    /// Provider implementations should chunk large lists and respect API rate limits.
    /// </remarks>
    Task<EmailResult> SendBulkAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        CancellationToken ct = default);
}