using MediatR;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using System.Net;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Handles administrator replies to Contact Us submissions.
/// </summary>
/// <remarks>
/// The reply is sent from the RCCG Hope House Info mailbox to the
/// email address supplied with the original Contact Us submission.
///
/// The Contact Us record is marked as responded only after the email
/// service confirms that the reply was sent successfully.
/// </remarks>
public sealed class ReplyToContactUsCommandHandler
    : IRequestHandler<ReplyToContactUsCommand, Unit>
{
    private readonly IContactUsRepository _repository;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ReplyToContactUsCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and update Contact Us submissions.
    /// </param>
    /// <param name="emailService">
    /// Service used to send the administrator's reply.
    /// </param>
    public ReplyToContactUsCommandHandler(
        IContactUsRepository repository,
        IEmailService emailService)
    {
        _repository =
            repository
            ?? throw new ArgumentNullException(
                nameof(repository));

        _emailService =
            emailService
            ?? throw new ArgumentNullException(
                nameof(emailService));
    }

    /// <summary>
    /// Sends an administrator's reply to the original Contact Us
    /// requester and marks the submission as responded when delivery
    /// succeeds.
    /// </summary>
    /// <param name="request">
    /// The reply command containing the Contact Us identifier,
    /// subject and reply body.
    /// </param>
    /// <param name="ct">
    /// A token used to observe cancellation.
    /// </param>
    /// <returns>
    /// <see cref="Unit.Value"/> when the reply has been sent and
    /// the Contact Us record has been updated.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified Contact Us submission does not exist.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the reply subject or body is empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the email service cannot send the reply.
    /// </exception>
    public async Task<Unit> Handle(
        ReplyToContactUsCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.Subject,
            nameof(request.Subject));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.Body,
            nameof(request.Body));

        var contact =
            await _repository.GetByIdAsync(
                request.ContactId,
                ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ContactUs),
                request.ContactId);

        var firstName =
            WebUtility.HtmlEncode(
                contact.FirstName);

        var replyBody =
            WebUtility.HtmlEncode(
                    request.Body.Trim())
                .Replace(
                    "\r\n",
                    "<br/>")
                .Replace(
                    "\n",
                    "<br/>");

        var htmlBody =
            $"""
             <p>Dear {firstName},</p>

             <p>
                 {replyBody}
             </p>

             <p>
                 Best regards,<br/>
                 RCCG Hope House Team
             </p>
             """;

        var emailResult =
            await _emailService.SendFromInfoAsync(
                contact.Email.Value,
                request.Subject.Trim(),
                htmlBody,
                ct);

        if (!emailResult.IsSuccess)
        {
            throw new InvalidOperationException(
                "The reply could not be sent from the RCCG Hope House " +
                $"Info mailbox. {emailResult.ErrorMessage}");
        }

        /*
         * A Contact Us submission must only be marked as responded
         * after the email service confirms successful delivery.
         */
        contact.MarkAsResponded();

        await _repository.UpdateAsync(
            contact,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        return Unit.Value;
    }
}