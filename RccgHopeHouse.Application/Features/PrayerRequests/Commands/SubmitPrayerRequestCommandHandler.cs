using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using System.Net;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Handles public prayer request submissions.
/// </summary>
/// <remarks>
/// The prayer request is persisted before email notifications are attempted.
/// This ensures that a temporary email delivery failure does not cause a
/// successfully submitted prayer request to be lost.
/// </remarks>
public sealed class SubmitPrayerRequestCommandHandler
    : IRequestHandler<SubmitPrayerRequestCommand, Unit>
{
    private readonly IPrayerRequestRepository _repository;
    private readonly IEmailService _emailService;
    private readonly NotificationSettings _settings;
    private readonly ILogger<SubmitPrayerRequestCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="SubmitPrayerRequestCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to persist prayer requests.
    /// </param>
    /// <param name="emailService">
    /// Service used to send prayer request email notifications.
    /// </param>
    /// <param name="settings">
    /// Notification settings used by the application.
    /// </param>
    /// <param name="logger">
    /// Logger used to record email delivery failures.
    /// </param>
    public SubmitPrayerRequestCommandHandler(
        IPrayerRequestRepository repository,
        IEmailService emailService,
        NotificationSettings settings,
        ILogger<SubmitPrayerRequestCommandHandler> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _settings = settings;
        _logger = logger;
    }

    /// <summary>
    /// Creates and persists a prayer request and then attempts
    /// the appropriate email notifications.
    /// </summary>
    /// <param name="request">
    /// The prayer request submission command.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// A MediatR unit result when processing is complete.
    /// </returns>
    public async Task<Unit> Handle(
        SubmitPrayerRequestCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var prayer = PrayerRequest.Create(
            content: request.Content,
            isAnonymous: request.IsAnonymous,
            requesterName: request.RequesterName,
            phoneNumber: request.PhoneNumber,
            requesterEmail: request.RequesterEmail);

        await _repository.AddAsync(
            prayer,
            ct);

        await _repository.SaveChangesAsync(
            ct);

        await NotifyPrayerTeamAsync(
            prayer,
            ct);

        if (
            !prayer.IsAnonymous &&
            prayer.RequesterEmail is not null
        )
        {
            await SendAcknowledgementAsync(
                prayer,
                ct);
        }

        return Unit.Value;
    }

    /// <summary>
    /// Sends an internal notification to the RCCG Hope House
    /// Info mailbox when a new prayer request is received.
    /// </summary>
    private async Task NotifyPrayerTeamAsync(
        PrayerRequest prayer,
        CancellationToken ct)
    {
        var requesterName =
            WebUtility.HtmlEncode(
                prayer.RequesterName);

        var prayerContent =
            ConvertTextToHtml(
                prayer.Content);

        var emailAddress =
            prayer.RequesterEmail is null
                ? null
                : WebUtility.HtmlEncode(
                    prayer.RequesterEmail.Value);

        var phoneNumber =
            prayer.PhoneNumber is null
                ? null
                : WebUtility.HtmlEncode(
                    prayer.PhoneNumber.Value);

        var subject =
            prayer.IsAnonymous
                ? "New Anonymous Prayer Request"
                : $"New Prayer Request from {prayer.RequesterName}";

        var htmlBody = $"""
            <h2>New Prayer Request</h2>

            <p>
                A new prayer request has been submitted through
                the RCCG Hope House website.
            </p>

            <p>
                <strong>Name:</strong>
                {requesterName}
                {(prayer.IsAnonymous
                    ? " (Anonymous)"
                    : string.Empty)}
            </p>

            {(emailAddress is null
                ? string.Empty
                : $"""
                   <p>
                       <strong>Email:</strong>
                       {emailAddress}
                   </p>
                   """)}

            {(phoneNumber is null
                ? string.Empty
                : $"""
                   <p>
                       <strong>Phone:</strong>
                       {phoneNumber}
                   </p>
                   """)}

            <p>
                <strong>Prayer Request:</strong>
            </p>

            <p>
                {prayerContent}
            </p>

            <p>
                <strong>Submitted:</strong>
                {prayer.CreatedAt:dd MMMM yyyy HH:mm}
            </p>

            <p>
                Please sign in to the RCCG Hope House
                administration panel to manage this request.
            </p>
            """;

        var replyTo =
            prayer.IsAnonymous
                ? null
                : prayer.RequesterEmail?.Value;

        var result =
            await _emailService.SendToInfoAsync(
                replyTo,
                subject,
                htmlBody,
                ct);

        if (!result.IsSuccess)
        {
            _logger.LogError(
                "Unable to send the prayer request notification " +
                "for prayer request {PrayerRequestId}. Error: {ErrorMessage}",
                prayer.Id,
                result.ErrorMessage);
        }
    }

    /// <summary>
    /// Sends an acknowledgement to a non-anonymous requester
    /// when an email address was supplied.
    /// </summary>
    private async Task SendAcknowledgementAsync(
        PrayerRequest prayer,
        CancellationToken ct)
    {
        if (prayer.RequesterEmail is null)
        {
            return;
        }

        var requesterName =
            WebUtility.HtmlEncode(
                prayer.RequesterName);

        var subject =
            "We Have Received Your Prayer Request";

        var htmlBody = $"""
            <p>
                Dear {requesterName},
            </p>

            <p>
                Thank you for sharing your prayer request
                with RCCG Hope House.
            </p>

            <p>
                Your request has been received and will be
                handled with care by our prayer team.
            </p>

            <p>
                May the Lord strengthen and encourage you.
            </p>

            <p>
                God bless you,<br/>
                <strong>RCCG Hope House</strong>
            </p>
            """;

        var result =
            await _emailService.SendFromInfoAsync(
                prayer.RequesterEmail.Value,
                subject,
                htmlBody,
                ct);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Prayer request {PrayerRequestId} was saved, " +
                "but the acknowledgement email could not be sent. " +
                "Error: {ErrorMessage}",
                prayer.Id,
                result.ErrorMessage);
        }
    }

    /// <summary>
    /// HTML-encodes plain text and preserves its line breaks
    /// for safe display in an HTML email.
    /// </summary>
    /// <param name="value">
    /// The plain-text value to convert.
    /// </param>
    /// <returns>
    /// HTML-safe text with line breaks converted to
    /// <c>&lt;br/&gt;</c> elements.
    /// </returns>
    private static string ConvertTextToHtml(
        string value)
    {
        return WebUtility
            .HtmlEncode(value)
            .Replace(
                "\r\n",
                "<br/>",
                StringComparison.Ordinal)
            .Replace(
                "\n",
                "<br/>",
                StringComparison.Ordinal);
    }
}