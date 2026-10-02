using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Handles pastoral workflow updates for prayer requests and
/// notifies the requester by email when the status changes.
/// </summary>
public sealed class UpdatePrayerRequestStatusCommandHandler
    : IRequestHandler<UpdatePrayerRequestStatusCommand, Unit>
{
    private readonly IPrayerRequestRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UpdatePrayerRequestStatusCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UpdatePrayerRequestStatusCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist prayer requests.
    /// </param>
    /// <param name="emailService">
    /// Email service used to send pastoral status notifications.
    /// </param>
    /// <param name="logger">
    /// Logger used to record notification failures without failing
    /// the persisted pastoral workflow update.
    /// </param>
    public UpdatePrayerRequestStatusCommandHandler(
        IPrayerRequestRepository repository,
        IEmailService emailService,
        ILogger<UpdatePrayerRequestStatusCommandHandler> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Updates the prayer request status and pastoral note.
    /// When the status genuinely changes and the requester supplied
    /// an email address, a pastoral update notification is sent after
    /// the database update has completed successfully.
    /// </summary>
    /// <param name="request">
    /// Command containing the prayer request identifier, new status,
    /// and optional pastoral note.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A MediatR unit result after the update has been processed.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the specified prayer request does not exist.
    /// </exception>
    public async Task<Unit> Handle(
        UpdatePrayerRequestStatusCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var prayer =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(PrayerRequest),
                request.Id);

        /*
         * Capture the existing status before applying the domain
         * update. This prevents duplicate emails when pastoral staff
         * save a note without actually changing the status.
         */
        var previousStatus =
            prayer.Status;

        var statusChanged =
            previousStatus !=
            request.NewStatus;

        /*
         * The existing domain method remains the authoritative place
         * for changing prayer status, pastoral note, and RespondedAt.
         */
        prayer.UpdateStatus(
            request.NewStatus,
            request.PastoralNote);

        await _repository.UpdateAsync(
            prayer,
            ct);

        /*
         * Persist the pastoral workflow update before attempting any
         * external email operation. An email problem must never cause
         * a successful status change to be lost.
         */
        await _repository.SaveChangesAsync(
            ct);

        if (
            statusChanged &&
            prayer.RequesterEmail is not null)
        {
            await TrySendStatusNotificationAsync(
                prayer,
                ct);
        }

        return Unit.Value;
    }

    /// <summary>
    /// Attempts to send a status-change notification to the requester.
    /// Email failures are logged rather than propagated because the
    /// prayer request update has already been persisted successfully.
    /// </summary>
    /// <param name="prayer">
    /// The updated prayer request.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    private async Task TrySendStatusNotificationAsync(
        PrayerRequest prayer,
        CancellationToken ct)
    {
        try
        {
            var subject =
                "Update on Your Prayer Request - RCCG Hope House";

            var htmlBody =
                BuildStatusNotification(
                    prayer);

            await _emailService.SendFromInfoAsync(
                prayer.RequesterEmail!.Value,
                subject,
                htmlBody,
                ct);
        }
        catch (Exception ex)
        {
            /*
             * Do not expose the requester's email address or prayer
             * content in application logs.
             */
            _logger.LogError(
                ex,
                "Prayer request {PrayerRequestId} was updated successfully, but the requester notification email could not be sent.",
                prayer.Id);
        }
    }

    /// <summary>
    /// Builds the pastoral notification email for the requester's
    /// current prayer request status.
    /// </summary>
    /// <param name="prayer">
    /// The updated prayer request.
    /// </param>
    /// <returns>
    /// HTML content suitable for sending to the requester.
    /// </returns>
    private static string BuildStatusNotification(
        PrayerRequest prayer)
    {
        var requesterName =
            WebUtility.HtmlEncode(
                prayer.RequesterName);

        var statusLabel =
            GetStatusLabel(
                prayer.Status);

        var statusMessage =
            GetStatusMessage(
                prayer.Status);

        var pastoralNoteSection =
            BuildPastoralNoteSection(
                prayer.PastoralNote);

        return $"""
            <div style="font-family: Arial, Helvetica, sans-serif; color: #344054; line-height: 1.7; max-width: 640px;">
                <h2 style="color: #101828;">
                    Update on Your Prayer Request
                </h2>

                <p>
                    Dear {requesterName},
                </p>

                <p>
                    Thank you for entrusting RCCG Hope House with your prayer request.
                </p>

                <div style="margin: 24px 0; padding: 16px; background: #f9fafb; border: 1px solid #eaecf0; border-radius: 8px;">
                    <strong style="color: #101828;">
                        Current Status: {statusLabel}
                    </strong>
                </div>

                <p>
                    {statusMessage}
                </p>

                {pastoralNoteSection}

                <p>
                    May the Lord strengthen you, give you peace, and answer you according to His will.
                </p>

                <p style="margin-top: 24px;">
                    With prayers and blessings,<br />
                    <strong>RCCG Hope House Pastoral Team</strong>
                </p>

                <p style="margin-top: 28px; color: #667085; font-size: 13px;">
                    This message was sent because you provided an email address with your prayer request.
                </p>
            </div>
            """;
    }

    /// <summary>
    /// Returns the display label for a prayer request status.
    /// </summary>
    private static string GetStatusLabel(
        PrayerRequestStatus status)
    {
        return status switch
        {
            PrayerRequestStatus.Pending =>
                "Pending",

            PrayerRequestStatus.InProgress =>
                "In Progress",

            PrayerRequestStatus.Resolved =>
                "Resolved",

            _ =>
                status.ToString()
        };
    }

    /// <summary>
    /// Returns the pastoral message associated with the current
    /// prayer request status.
    /// </summary>
    private static string GetStatusMessage(
        PrayerRequestStatus status)
    {
        return status switch
        {
            PrayerRequestStatus.Pending =>
                "Your prayer request is currently awaiting pastoral attention. Please be assured that it has been received by our team.",

            PrayerRequestStatus.InProgress =>
                "Your prayer request is currently being attended to by our pastoral and prayer team. Please be assured that we are standing with you in prayer.",

            PrayerRequestStatus.Resolved =>
                "Your prayer request has now been marked as resolved by our pastoral and prayer team. We continue to trust God with you and pray that His grace, peace, and guidance will remain with you.",

            _ =>
                "There has been an update to your prayer request."
        };
    }

    /// <summary>
    /// Builds the optional pastoral note section of the notification.
    /// User-entered content is HTML encoded before being included
    /// in the email.
    /// </summary>
    private static string BuildPastoralNoteSection(
        string? pastoralNote)
    {
        if (
            string.IsNullOrWhiteSpace(
                pastoralNote))
        {
            return string.Empty;
        }

        var encodedNote =
            WebUtility.HtmlEncode(
                pastoralNote);

        encodedNote =
            encodedNote
                .Replace(
                    "\r\n",
                    "<br />")
                .Replace(
                    "\n",
                    "<br />");

        return $"""
            <div style="margin: 24px 0; padding: 18px; background: #f8f9fc; border-left: 4px solid #475467; border-radius: 6px;">
                <strong style="display: block; margin-bottom: 8px; color: #101828;">
                    Message from the Pastoral Team
                </strong>

                <div>
                    {encodedNote}
                </div>
            </div>
            """;
    }
}