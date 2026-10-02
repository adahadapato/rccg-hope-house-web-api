using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Provides SMTP-based email delivery for RCCG Hope House.
/// </summary>
/// <remarks>
/// The service supports two configured RCCG Hope House mailboxes:
/// the Admin mailbox for system/administrative communication and the
/// Info mailbox for public-facing communication.
///
/// Public form submissions are always sent from an authenticated
/// RCCG Hope House mailbox. A visitor's email address is added as
/// Reply-To rather than being used as the From address. This improves
/// SMTP authentication compatibility and allows staff to reply directly
/// to the visitor.
/// </remarks>
public sealed class EmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    private readonly string _adminUsername;
    private readonly string _adminPassword;

    private readonly string _infoUsername;
    private readonly string _infoPassword;

    private readonly MailAddress _adminFromAddress;
    private readonly MailAddress _infoFromAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class
    /// using values from the <c>EmailSettings</c> configuration section.
    /// </summary>
    /// <param name="configuration">
    /// The application configuration containing SMTP and mailbox settings.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a required email configuration value is missing or invalid.
    /// </exception>
    public EmailService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _smtpHost =
            configuration["EmailSettings:SmtpHost"]
            ?? throw new InvalidOperationException(
                "EmailSettings:SmtpHost is not configured.");

        var smtpPortValue =
            configuration["EmailSettings:SmtpPort"]
            ?? throw new InvalidOperationException(
                "EmailSettings:SmtpPort is not configured.");

        if (!int.TryParse(smtpPortValue, out _smtpPort))
        {
            throw new InvalidOperationException(
                "EmailSettings:SmtpPort must be a valid number.");
        }

        _adminUsername =
            configuration["EmailSettings:AdminUsername"]
            ?? throw new InvalidOperationException(
                "EmailSettings:AdminUsername is not configured.");

        _adminPassword =
            configuration["EmailSettings:AdminPassword"]
            ?? throw new InvalidOperationException(
                "EmailSettings:AdminPassword is not configured.");

        _infoUsername =
            configuration["EmailSettings:InfoUsername"]
            ?? throw new InvalidOperationException(
                "EmailSettings:InfoUsername is not configured.");

        _infoPassword =
            configuration["EmailSettings:InfoPassword"]
            ?? throw new InvalidOperationException(
                "EmailSettings:InfoPassword is not configured.");

        var defaultFromAddress =
            configuration["EmailSettings:DefaultFromAddress"]
            ?? _adminUsername;

        var defaultFromName =
            configuration["EmailSettings:DefaultFromName"]
            ?? "RCCG Hope House";

        _adminFromAddress =
            new MailAddress(
                defaultFromAddress,
                defaultFromName);

        _infoFromAddress =
            new MailAddress(
                _infoUsername,
                defaultFromName);
    }

    /// <inheritdoc />
    public Task<EmailResult> SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        return SendInternalAsync(
            to,
            subject,
            htmlBody,
            _adminUsername,
            _adminPassword,
            _adminFromAddress,
            replyTo: null,
            ct);
    }

    /// <inheritdoc />
    public Task<EmailResult> SendToAdminAsync(
        string? replyTo,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        return SendInternalAsync(
            _adminFromAddress.Address,
            subject,
            htmlBody,
            _adminUsername,
            _adminPassword,
            _adminFromAddress,
            replyTo,
            ct);
    }

    /// <inheritdoc />
    public Task<EmailResult> SendToInfoAsync(
        string? replyTo,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        return SendInternalAsync(
            _infoFromAddress.Address,
            subject,
            htmlBody,
            _infoUsername,
            _infoPassword,
            _infoFromAddress,
            replyTo,
            ct);
    }

    /// <inheritdoc />
    public Task<EmailResult> SendFromAdminAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        return SendInternalAsync(
            to,
            subject,
            htmlBody,
            _adminUsername,
            _adminPassword,
            _adminFromAddress,
            replyTo: null,
            ct);
    }

    /// <inheritdoc />
    public Task<EmailResult> SendFromInfoAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        return SendInternalAsync(
            to,
            subject,
            htmlBody,
            _infoUsername,
            _infoPassword,
            _infoFromAddress,
            replyTo: null,
            ct);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Each recipient receives an individual message. This prevents
    /// recipient addresses from being exposed to other mailing-list
    /// subscribers.
    ///
    /// Messages are sent sequentially to avoid opening a large number
    /// of simultaneous SMTP connections.
    /// </remarks>
    public async Task<EmailResult> SendBulkAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(recipients);

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Failure(
                "Email subject is required.");
        }

        if (string.IsNullOrWhiteSpace(htmlBody))
        {
            return Failure(
                "Email body is required.");
        }

        var recipientList =
            recipients
                .Where(recipient =>
                    !string.IsNullOrWhiteSpace(
                        recipient))
                .Select(recipient =>
                    recipient.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        if (recipientList.Count == 0)
        {
            return Failure(
                "At least one recipient email address is required.");
        }

        var sentCount = 0;

        foreach (var recipient in recipientList)
        {
            ct.ThrowIfCancellationRequested();

            var result =
                await SendInternalAsync(
                    recipient,
                    subject,
                    htmlBody,
                    _infoUsername,
                    _infoPassword,
                    _infoFromAddress,
                    replyTo: null,
                    ct);

            if (!result.IsSuccess)
            {
                return new EmailResult(
                    IsSuccess: false,
                    MessageId: null,
                    ErrorMessage:
                        $"Bulk email stopped after " +
                        $"{sentCount} successful message(s). " +
                        $"Failed recipient: {recipient}. " +
                        $"{result.ErrorMessage}");
            }

            sentCount++;
        }

        return new EmailResult(
            IsSuccess: true,
            MessageId:
                $"bulk-{sentCount}-{Guid.NewGuid():N}",
            ErrorMessage: null);
    }

    /// <summary>
    /// Sends a single HTML email using the supplied authenticated mailbox.
    /// </summary>
    /// <param name="to">
    /// The recipient's email address.
    /// </param>
    /// <param name="subject">
    /// The email subject.
    /// </param>
    /// <param name="htmlBody">
    /// The HTML body of the email.
    /// </param>
    /// <param name="username">
    /// The SMTP username used to authenticate the sending mailbox.
    /// </param>
    /// <param name="password">
    /// The SMTP password used to authenticate the sending mailbox.
    /// </param>
    /// <param name="fromAddress">
    /// The authenticated RCCG Hope House address displayed as the sender.
    /// </param>
    /// <param name="replyTo">
    /// Optional address to which replies should be directed.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation while the message is being sent.
    /// </param>
    /// <returns>
    /// An <see cref="EmailResult"/> describing the send result.
    /// </returns>
    private async Task<EmailResult> SendInternalAsync(
        string to,
        string subject,
        string htmlBody,
        string username,
        string password,
        MailAddress fromAddress,
        string? replyTo,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            return Failure(
                "Recipient email address is required.");
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Failure(
                "Email subject is required.");
        }

        if (string.IsNullOrWhiteSpace(htmlBody))
        {
            return Failure(
                "Email body is required.");
        }

        try
        {
            ct.ThrowIfCancellationRequested();

            using var smtpClient =
                CreateSmtpClient(
                    username,
                    password);

            using var message =
                new MailMessage
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

            message.To.Add(
                new MailAddress(to));

            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                message.ReplyToList.Add(
                    new MailAddress(replyTo));
            }

            await smtpClient.SendMailAsync(
                message,
                ct);

            return new EmailResult(
                IsSuccess: true,
                MessageId:
                    Guid.NewGuid().ToString("N"),
                ErrorMessage: null);
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (FormatException ex)
        {
            return Failure(
                $"Invalid email address: {ex.Message}");
        }
        catch (SmtpException ex)
        {
            return Failure(
                $"SMTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Failure(
                ex.Message);
        }
    }

    /// <summary>
    /// Creates an SMTP client authenticated with the specified
    /// RCCG Hope House mailbox.
    /// </summary>
    /// <param name="username">
    /// The SMTP account username.
    /// </param>
    /// <param name="password">
    /// The SMTP account password.
    /// </param>
    /// <returns>
    /// A configured <see cref="SmtpClient"/>.
    /// </returns>
    private SmtpClient CreateSmtpClient(
        string username,
        string password)
    {
        return new SmtpClient(
            _smtpHost,
            _smtpPort)
        {
            EnableSsl = true,
            DeliveryMethod =
                SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials =
                new NetworkCredential(
                    username,
                    password)
        };
    }

    /// <summary>
    /// Creates an unsuccessful email result.
    /// </summary>
    /// <param name="errorMessage">
    /// A human-readable description of the failure.
    /// </param>
    /// <returns>
    /// A failed <see cref="EmailResult"/>.
    /// </returns>
    private static EmailResult Failure(
        string errorMessage)
    {
        return new EmailResult(
            IsSuccess: false,
            MessageId: null,
            ErrorMessage: errorMessage);
    }
}