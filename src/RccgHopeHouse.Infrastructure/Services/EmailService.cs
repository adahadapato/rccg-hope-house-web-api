using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements <see cref="IEmailService"/> using SMTP.
/// Suitable for transactional emails, admin alerts, and pastoral notifications.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly MailAddress _fromAddress;

    /// <summary>
    /// Initializes the SMTP client with configuration settings.
    /// </summary>
    public EmailService(IConfiguration configuration)
    {
        var host = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        var port = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        var username = configuration["Email:Username"];
        var password = configuration["Email:Password"];
        var fromEmail = configuration["Email:FromAddress"] ?? "noreply@rccghopehouse.org.uk";
        var fromName = configuration["Email:FromName"] ?? "RCCG Hope House";

        _smtpClient = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        _fromAddress = new MailAddress(fromEmail, fromName);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Wraps SMTP calls in try/catch to return EmailResult instead of throwing.
    /// Prevents unhandled exceptions from breaking HTTP requests during fire-and-forget notifications.
    /// </remarks>
    public async Task<EmailResult> SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        try
        {
            var message = new MailMessage(_fromAddress, new MailAddress(to))
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(message, ct);
            return new EmailResult(true, Guid.NewGuid().ToString(), null);
        }
        catch (Exception ex)
        {
            return new EmailResult(false, null, ex.Message);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Sends bulk emails sequentially to respect SMTP rate limits.
    /// For high-volume newsletters, consider migrating to SendGrid/Mailgun API.
    /// </remarks>
    public async Task<EmailResult> SendBulkAsync(IEnumerable<string> recipients, string subject, string htmlBody, CancellationToken ct = default)
    {
        foreach (var to in recipients)
        {
            var result = await SendAsync(to, subject, htmlBody, ct);
            if (!result.IsSuccess) return result;
        }
        return new EmailResult(true, "bulk-sent", null);
    }
}