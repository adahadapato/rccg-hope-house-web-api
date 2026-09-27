using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IEmailService"/> using SMTP.
/// Suitable for transactional emails, admin alerts,
/// and pastoral notifications.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly MailAddress _fromAddress;

    /// <summary>
    /// Initializes the SMTP client using the
    /// EmailSettings configuration section.
    /// </summary>
    public EmailService(
        IConfiguration configuration)
    {
        var host =
            configuration[
                "EmailSettings:SmtpHost"]
            ?? "smtp.gmail.com";

        var portValue =
            configuration[
                "EmailSettings:SmtpPort"]
            ?? "587";

        if (!int.TryParse(
                portValue,
                out var port))
        {
            throw new InvalidOperationException(
                "EmailSettings:SmtpPort must be a valid number.");
        }

        var username =
            configuration[
                "EmailSettings:Username"];

        var password =
            configuration[
                "EmailSettings:Password"];

        var fromEmail =
            configuration[
                "EmailSettings:FromAddress"]
            ?? "noreply@rccghopehouse.org.uk";

        var fromName =
            configuration[
                "EmailSettings:FromName"]
            ?? "RCCG Hope House";

        _smtpClient =
            new SmtpClient(
                host,
                port)
            {
                EnableSsl = true,
                DeliveryMethod =
                    SmtpDeliveryMethod.Network,
                UseDefaultCredentials =
                    false
            };

        if (
            !string.IsNullOrWhiteSpace(
                username) &&
            !string.IsNullOrWhiteSpace(
                password)
        )
        {
            _smtpClient.Credentials =
                new NetworkCredential(
                    username,
                    password);
        }

        _fromAddress =
            new MailAddress(
                fromEmail,
                fromName);
    }

    /// <inheritdoc />
    public async Task<EmailResult> SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        try
        {
            using var message =
                new MailMessage
                {
                    From = _fromAddress,
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

            message.To.Add(
                new MailAddress(to));

            await _smtpClient.SendMailAsync(
                message,
                ct);

            return new EmailResult(
                IsSuccess: true,
                MessageId:
                    Guid.NewGuid()
                        .ToString(),
                ErrorMessage: null);
        }
        catch (OperationCanceledException)
            when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new EmailResult(
                IsSuccess: false,
                MessageId: null,
                ErrorMessage:
                    ex.Message);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Sends bulk emails sequentially to respect
    /// SMTP rate limits.
    /// </remarks>
    public async Task<EmailResult> SendBulkAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        CancellationToken ct = default)
    {
        foreach (var recipient in recipients)
        {
            ct.ThrowIfCancellationRequested();

            var result =
                await SendAsync(
                    recipient,
                    subject,
                    htmlBody,
                    ct);

            if (!result.IsSuccess)
            {
                return result;
            }
        }

        return new EmailResult(
            IsSuccess: true,
            MessageId: "bulk-sent",
            ErrorMessage: null);
    }
}