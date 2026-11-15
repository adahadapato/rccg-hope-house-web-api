using MediatR;
using Microsoft.Extensions.Logging;
using RccgHopeHouse.Application.Features.FirstTimers.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using System.Net;

namespace RccgHopeHouse.Application.Features.FirstTimers.Commands;

/// <summary>
/// The public welcome form, opened from the QR code.
/// </summary>
public record SubmitFirstTimerCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? BirthMonth,
    int? BirthDay,
    FirstTimerSource? Source,
    string? InvitedBy,
    bool WantsPastoralContact,
    FirstTimerContactMethod PreferredContactMethod,
    string? PrayerRequest,
    string? Comment,
    bool ConsentToContact)
    : IRequest<SubmitFirstTimerResultDto>;

public sealed class SubmitFirstTimerCommandHandler
    : IRequestHandler<SubmitFirstTimerCommand, SubmitFirstTimerResultDto>
{
    private const string ThankYouMessage =
        "Thank you for worshipping with us today. We are so glad you came!";

    private readonly IFirstTimerRepository _repository;
    private readonly IPrayerRequestRepository _prayerRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<SubmitFirstTimerCommandHandler> _logger;

    public SubmitFirstTimerCommandHandler(
        IFirstTimerRepository repository,
        IPrayerRequestRepository prayerRepository,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<SubmitFirstTimerCommandHandler> logger)
    {
        _repository = repository;
        _prayerRepository = prayerRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<SubmitFirstTimerResultDto> Handle(
        SubmitFirstTimerCommand request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var visitDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var existing = await _repository.GetByNormalizedPhoneAsync(
            FirstTimer.NormalizePhone(request.PhoneNumber),
            ct);

        // The prayer request is saved as a normal prayer request so
        // that only the prayer team can read it.
        PrayerRequest? prayer = null;

        if (!string.IsNullOrWhiteSpace(request.PrayerRequest))
        {
            prayer = PrayerRequest.Create(
                content: request.PrayerRequest,
                isAnonymous: false,
                requesterName: $"{request.FirstName.Trim()} {request.LastName.Trim()}",
                phoneNumber: request.PhoneNumber,
                requesterEmail: request.Email);

            await _prayerRepository.AddAsync(prayer, ct);
        }

        FirstTimer firstTimer;
        var isNew = existing is null;

        if (existing is null)
        {
            firstTimer = FirstTimer.Create(
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber,
                consentToContact: request.ConsentToContact,
                preferredContactMethod: request.PreferredContactMethod,
                visitDate: visitDate,
                email: request.Email,
                addressLine1: request.AddressLine1,
                addressLine2: request.AddressLine2,
                city: request.City,
                postcode: request.Postcode,
                birthMonth: request.BirthMonth,
                birthDay: request.BirthDay,
                source: request.Source,
                invitedBy: request.InvitedBy,
                wantsPastoralContact: request.WantsPastoralContact,
                comment: request.Comment);

            await _repository.AddAsync(firstTimer, ct);
        }
        else
        {
            // Same phone number as before: record the visit, but leave
            // the details already on file untouched.
            firstTimer = existing;
            firstTimer.RecordReturnVisit(visitDate, request.Comment);
        }

        if (prayer is not null)
        {
            firstTimer.LinkPrayerRequest(prayer.Id);
        }

        // Both repositories share one database context, so the visitor
        // and the prayer request are saved together.
        await _repository.SaveChangesAsync(ct);

        if (isNew || prayer is not null)
        {
            await NotifyChurchAsync(firstTimer, isNew, prayer is not null, ct);
        }

        if (isNew)
        {
            await SendThankYouAsync(firstTimer, ct);
        }

        return new SubmitFirstTimerResultDto(ThankYouMessage);
    }

    // ================================================================
    // Notification to the church inbox
    // ================================================================

    private async Task NotifyChurchAsync(
        FirstTimer firstTimer,
        bool isNew,
        bool hasPrayerRequest,
        CancellationToken ct)
    {
        var name = WebUtility.HtmlEncode(
            $"{firstTimer.FirstName} {firstTimer.LastName}");

        var subject = isNew
            ? $"New first-time visitor: {firstTimer.FirstName} {firstTimer.LastName}"
            : $"Returning visitor sent a prayer request: {firstTimer.FirstName} {firstTimer.LastName}";

        // The prayer request text is deliberately left out of this
        // email. It is confidential and is read in the admin area.
        var htmlBody = $"""
            <h2>{(isNew ? "New First-Time Visitor" : "Returning Visitor")}</h2>

            <p>
                <strong>Name:</strong> {name}
            </p>

            <p>
                <strong>Phone:</strong>
                {WebUtility.HtmlEncode(firstTimer.PhoneNumber)}
            </p>

            {(firstTimer.Email is null
                ? string.Empty
                : $"""
                   <p>
                       <strong>Email:</strong>
                       {WebUtility.HtmlEncode(firstTimer.Email)}
                   </p>
                   """)}

            <p>
                <strong>Preferred contact:</strong>
                {firstTimer.PreferredContactMethod}
            </p>

            <p>
                <strong>Would like a call or visit:</strong>
                {(firstTimer.WantsPastoralContact ? "Yes" : "No")}
            </p>

            {(hasPrayerRequest
                ? """
                  <p>
                      <strong>A confidential prayer request was included.</strong>
                      The prayer team can read it under Prayer Requests
                      in the administration panel.
                  </p>
                  """
                : string.Empty)}

            <p>
                Please sign in to the RCCG Hope House administration
                panel to follow up.
            </p>
            """;

        try
        {
            var result = await _emailService.SendToInfoAsync(
                replyTo: null,
                subject,
                htmlBody,
                ct);

            if (!result.IsSuccess)
            {
                _logger.LogError(
                    "Unable to send the first-timer notification for {FirstTimerId}. " +
                    "Error: {ErrorMessage}",
                    firstTimer.Id,
                    result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            // The visitor is already saved; never fail the form over email.
            _logger.LogError(
                ex,
                "Unexpected error sending the first-timer notification for {FirstTimerId}.",
                firstTimer.Id);
        }
    }

    // ================================================================
    // Thank-you to the visitor
    // ================================================================
    //
    // Email when they gave an address AND chose email as their
    // contact method. Otherwise (or if the email fails) a text message.

    private async Task SendThankYouAsync(
        FirstTimer firstTimer,
        CancellationToken ct)
    {
        var wantsEmail =
            firstTimer.Email is not null &&
            firstTimer.PreferredContactMethod == FirstTimerContactMethod.Email;

        if (wantsEmail && await TrySendThankYouEmailAsync(firstTimer, ct))
        {
            return;
        }

        await TrySendThankYouSmsAsync(firstTimer, ct);
    }

    private async Task<bool> TrySendThankYouEmailAsync(
        FirstTimer firstTimer,
        CancellationToken ct)
    {
        var firstName = WebUtility.HtmlEncode(firstTimer.FirstName);

        var htmlBody = $"""
            <p>
                Dear {firstName},
            </p>

            <p>
                Thank you for worshipping with us at RCCG Hope House.
                It was a joy to have you, and we hope you felt at home.
            </p>

            <p>
                We would love to see you again. If there is anything
                we can do for you, simply reply to this email.
            </p>

            <p>
                God bless you,<br/>
                <strong>RCCG Hope House</strong>
            </p>
            """;

        try
        {
            var result = await _emailService.SendFromInfoAsync(
                firstTimer.Email!,
                "Thank You for Worshipping with Us",
                htmlBody,
                ct);

            if (result.IsSuccess)
            {
                return true;
            }

            _logger.LogWarning(
                "First-timer {FirstTimerId} was saved, but the thank-you email " +
                "could not be sent. Error: {ErrorMessage}",
                firstTimer.Id,
                result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Unexpected error sending the thank-you email for first-timer {FirstTimerId}.",
                firstTimer.Id);
        }

        return false;
    }

    private async Task TrySendThankYouSmsAsync(
        FirstTimer firstTimer,
        CancellationToken ct)
    {
        var message =
            $"Dear {firstTimer.FirstName}, thank you for worshipping with us " +
            "at RCCG Hope House today. It was a joy to have you and we " +
            "hope to see you again soon. God bless you!";

        try
        {
            var phoneNumber =
                Core.ValueObjects.PhoneNumber
                    .Create(firstTimer.PhoneNumber)
                    .ToE164();

            var result = await _smsService.SendAsync(phoneNumber, message, ct);

            if (!result.IsSuccessful)
            {
                _logger.LogWarning(
                    "First-timer {FirstTimerId} was saved, but the thank-you text " +
                    "was rejected. Provider status: {ProviderStatus}; error: {ProviderError}.",
                    firstTimer.Id,
                    result.Status,
                    result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            // No phone number is logged.
            _logger.LogWarning(
                ex,
                "First-timer {FirstTimerId} was saved, but the thank-you text could not be sent.",
                firstTimer.Id);
        }
    }
}
