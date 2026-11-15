namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Represents the result of an email delivery attempt.
/// </summary>
/// <param name="IsSuccess">
/// Indicates whether the email was successfully submitted to the configured
/// email transport.
/// </param>
/// <param name="MessageId">
/// An identifier for the email operation when available.
/// </param>
/// <param name="ErrorMessage">
/// A human-readable error message when the email could not be sent.
/// </param>
public record EmailResult(
    bool IsSuccess,
    string? MessageId,
    string? ErrorMessage);

/// <summary>
/// Defines email operations used by the RCCG Hope House application.
/// </summary>
/// <remarks>
/// The application uses separate Admin and Info mailboxes so that system
/// notifications and public-facing communication can be routed through
/// the appropriate RCCG Hope House email account.
/// </remarks>
public interface IEmailService
{
    /// <summary>
    /// Sends a generic transactional email using the default Admin mailbox.
    /// </summary>
    /// <param name="to">
    /// The recipient's email address.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the email is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the result of the send operation.
    /// </returns>
    Task<EmailResult> SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends individual emails to multiple recipients using the Info mailbox.
    /// </summary>
    /// <remarks>
    /// Each recipient receives a separate message so recipient addresses
    /// are not exposed to other recipients.
    /// </remarks>
    /// <param name="recipients">
    /// The email addresses that should receive the message.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while emails are being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the overall bulk-send result.
    /// </returns>
    Task<EmailResult> SendBulkAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends an internal notification to the Admin mailbox.
    /// </summary>
    /// <remarks>
    /// The email is sent from the configured RCCG Hope House Admin mailbox.
    /// When <paramref name="replyTo"/> is supplied, replies from the Admin
    /// inbox are directed to that address.
    /// </remarks>
    /// <param name="replyTo">
    /// Optional email address that should receive replies to the message.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the email is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the result of the send operation.
    /// </returns>
    Task<EmailResult> SendToAdminAsync(
        string? replyTo,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends an internal notification to the Info mailbox.
    /// </summary>
    /// <remarks>
    /// This operation is intended for public-facing forms such as Contact Us.
    /// The email is sent from the configured RCCG Hope House Info mailbox
    /// to the same Info mailbox.
    ///
    /// When <paramref name="replyTo"/> contains the visitor's email address,
    /// church staff can use the normal Reply action in their email client
    /// and the response will be addressed directly to the visitor.
    /// </remarks>
    /// <param name="replyTo">
    /// Optional email address that should receive replies to the message.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the email is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the result of the send operation.
    /// </returns>
    Task<EmailResult> SendToInfoAsync(
        string? replyTo,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends an email to an external recipient using the Admin mailbox.
    /// </summary>
    /// <param name="to">
    /// The recipient's email address.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the email is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the result of the send operation.
    /// </returns>
    Task<EmailResult> SendFromAdminAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default);

    /// <summary>
    /// Sends an email to an external recipient using the Info mailbox.
    /// </summary>
    /// <remarks>
    /// This operation is suitable for acknowledgements and other
    /// public-facing correspondence sent by RCCG Hope House.
    /// </remarks>
    /// <param name="to">
    /// The recipient's email address.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML content of the email.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the email is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the result of the send operation.
    /// </returns>
    Task<EmailResult> SendFromInfoAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default);
}