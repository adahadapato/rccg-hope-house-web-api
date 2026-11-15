using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.Interfaces;
using System.Net;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Handles public Contact Us form submissions.
/// </summary>
/// <remarks>
/// A successful submission is persisted before email delivery is
/// attempted so that the church retains the enquiry even when the
/// configured email transport is temporarily unavailable.
///
/// After persistence, the handler:
/// <list type="number">
/// <item>
/// <description>
/// Sends the submitted enquiry to the RCCG Hope House Info mailbox.
/// </description>
/// </item>
/// <item>
/// <description>
/// Sends an acknowledgement from the Info mailbox to the visitor.
/// </description>
/// </item>
/// </list>
///
/// The visitor's email address is supplied as the Reply-To address on
/// the internal notification so church staff can reply directly to
/// the visitor from the Info inbox.
/// </remarks>
public sealed class SubmitContactUsCommandHandler
    : IRequestHandler<SubmitContactUsCommand, Unit>
{
    private readonly IContactUsRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<SubmitContactUsCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="SubmitContactUsCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to persist Contact Us submissions.
    /// </param>
    /// <param name="emailService">
    /// Service used to deliver the internal notification and
    /// visitor acknowledgement emails.
    /// </param>
    /// <param name="logger">
    /// Logger used to record email-delivery failures without
    /// discarding an already persisted Contact Us submission.
    /// </param>
    public SubmitContactUsCommandHandler(
        IContactUsRepository repository,
        IEmailService emailService,
        ILogger<SubmitContactUsCommandHandler> logger)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));

        _emailService =
            emailService
            ?? throw new ArgumentNullException(
                nameof(emailService));

        _logger =
            logger
            ?? throw new ArgumentNullException(
                nameof(logger));
    }

    /// <summary>
    /// Saves the Contact Us submission, notifies the RCCG Hope House
    /// Info mailbox and sends a confirmation email to the visitor.
    /// </summary>
    /// <param name="request">
    /// The submitted Contact Us request.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation.
    /// </param>
    /// <returns>
    /// <see cref="Unit.Value"/> when processing has completed.
    /// </returns>
    public async Task<Unit> Handle(
        SubmitContactUsCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(
            request);

        var contact =
            Core.Entities.ContactUs.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Message,
                request.Reason,
                request.PhoneNumber);

        await _repository.AddAsync(
            contact,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        /*
         * Persist before attempting email delivery.
         *
         * If SMTP is temporarily unavailable, the Contact Us
         * submission must remain available in the administrative
         * inbox and must not be lost.
         */
        var infoNotificationTask =
            NotifyInfoMailboxAsync(
                contact,
                ct);

        var visitorAcknowledgementTask =
            NotifySenderAsync(
                contact,
                ct);

        await Task.WhenAll(
            infoNotificationTask,
            visitorAcknowledgementTask);

        return Unit.Value;
    }

    /// <summary>
    /// Sends the submitted Contact Us enquiry to the RCCG Hope House
    /// Info mailbox.
    /// </summary>
    /// <remarks>
    /// The message is sent from the authenticated Info mailbox to the
    /// Info mailbox. The visitor's email address is configured as
    /// Reply-To so staff can reply directly without using the
    /// visitor's address as the SMTP From address.
    /// </remarks>
    /// <param name="contact">
    /// The persisted Contact Us submission.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation.
    /// </param>
    private async Task NotifyInfoMailboxAsync(
        Core.Entities.ContactUs contact,
        CancellationToken ct)
    {
        var subject =
            $"New Contact Request: {contact.Reason}";

        var firstName =
            WebUtility.HtmlEncode(
                contact.FirstName);

        var lastName =
            WebUtility.HtmlEncode(
                contact.LastName);

        var email =
            WebUtility.HtmlEncode(
                contact.Email.Value);

        var reason =
            WebUtility.HtmlEncode(
                contact.Reason.ToString());

        var message =
            ConvertTextToHtml(
                contact.Message);

        var phoneHtml =
            contact.PhoneNumber is not null
                ? $"""
                   <p>
                       <strong>Phone:</strong>
                       {WebUtility.HtmlEncode(contact.PhoneNumber.Value)}
                   </p>
                   """
                : string.Empty;

        var htmlBody =
            $"""
             <h3>New Contact Form Submission</h3>

             <p>
                 <strong>Name:</strong>
                 {firstName} {lastName}
             </p>

             <p>
                 <strong>Email:</strong>
                 {email}
             </p>

             {phoneHtml}

             <p>
                 <strong>Reason:</strong>
                 {reason}
             </p>

             <p>
                 <strong>Message:</strong><br/>
                 {message}
             </p>

             <p>
                 <em>
                     Submitted on
                     {contact.CreatedAt:MMMM dd, yyyy 'at' HH:mm}
                 </em>
             </p>
             """;

        var result =
            await _emailService.SendToInfoAsync(
                contact.Email.Value,
                subject,
                htmlBody,
                ct);

        if (!result.IsSuccess)
        {
            /*
             * The submission has already been stored successfully.
             * Record the notification failure without invalidating
             * or deleting the Contact Us submission.
             */
            _logger.LogError(
                "Failed to send Contact Us notification for contact {ContactId} " +
                "to the Info mailbox. Error: {ErrorMessage}",
                contact.Id,
                result.ErrorMessage);
        }
    }

    /// <summary>
    /// Sends an acknowledgement email to the visitor confirming that
    /// RCCG Hope House has received the Contact Us submission.
    /// </summary>
    /// <param name="contact">
    /// The persisted Contact Us submission.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation.
    /// </param>
    private async Task NotifySenderAsync(
        Core.Entities.ContactUs contact,
        CancellationToken ct)
    {
        const string subject =
            "Thank you for contacting RCCG Hope House";

        var firstName =
            WebUtility.HtmlEncode(
                contact.FirstName);

        var submittedMessage =
            ConvertTextToHtml(
                contact.Message);

        var htmlBody =
            $"""
             <h3>Hello {firstName},</h3>

             <p>
                 Thank you for contacting RCCG Hope House.
                 We have successfully received your message.
             </p>

             <p>
                 A member of our team will review your enquiry
                 and get back to you as soon as possible.
             </p>

             <p>
                 <strong>
                     For your reference, this is the message
                     we received:
                 </strong>
             </p>

             <blockquote
                 style="border-left: 3px solid #ccc;
                        padding-left: 10px;
                        color: #555;">
                 {submittedMessage}
             </blockquote>

             <p>God bless you!</p>

             <p>
                 <em>RCCG Hope House</em>
             </p>
             """;

        var result =
            await _emailService.SendFromInfoAsync(
                contact.Email.Value,
                subject,
                htmlBody,
                ct);

        if (!result.IsSuccess)
        {
            /*
             * Failure to send the acknowledgement must not remove
             * or invalidate the already persisted submission.
             */
            _logger.LogWarning(
                "Contact Us submission {ContactId} was stored, but the " +
                "visitor acknowledgement could not be sent to {EmailAddress}. " +
                "Error: {ErrorMessage}",
                contact.Id,
                contact.Email.Value,
                result.ErrorMessage);
        }
    }

    /// <summary>
    /// HTML-encodes plain text and preserves line breaks for safe
    /// inclusion in an HTML email.
    /// </summary>
    /// <param name="value">
    /// Plain text to convert.
    /// </param>
    /// <returns>
    /// HTML-safe text with line breaks represented by
    /// <c>&lt;br/&gt;</c>.
    /// </returns>
    private static string ConvertTextToHtml(
        string value)
    {
        return WebUtility.HtmlEncode(
                value)
            .Replace(
                "\r\n",
                "<br/>")
            .Replace(
                "\n",
                "<br/>");
    }
}