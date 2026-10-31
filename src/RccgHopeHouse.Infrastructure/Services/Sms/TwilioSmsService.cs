
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;
using RccgHopeHouse.Core.ValueObjects;
using Twilio.Clients;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace RccgHopeHouse.Infrastructure.Services.Sms;

/// <summary>
/// Provides SMS messaging functionality through the Twilio
/// Messaging API and builds secure celebration invitation URLs.
/// </summary>
/// <remarks>
/// Retrieves Twilio credentials, sender information and the
/// public website URL from application configuration.
///
/// The Twilio authentication token should be stored in
/// .NET User Secrets during development and supplied through
/// secure environment configuration in production.
///
/// Website URLs are validated using the existing SafeUrl
/// value object from the Core layer.
/// </remarks>
public sealed class TwilioSmsService : ISmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _from;
    private readonly string _websiteBaseUrl;

    /// <summary>
    /// Initialises the Twilio SMS service.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration containing Twilio and website settings.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required configuration is missing or invalid.
    /// </exception>
    public TwilioSmsService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _accountSid = configuration[AppSettings.Twilio.AccountSid]
            ?? throw new InvalidOperationException(
                "Twilio Account SID is not configured.");

        _authToken = configuration[AppSettings.Twilio.AuthToken]
            ?? throw new InvalidOperationException(
                "Twilio Auth Token is not configured.");

        _from = configuration[AppSettings.Twilio.From]
            ?? throw new InvalidOperationException(
                "Twilio sender number is not configured.");

        if (string.IsNullOrWhiteSpace(_accountSid) ||
            string.IsNullOrWhiteSpace(_authToken) ||
            string.IsNullOrWhiteSpace(_from))
        {
            throw new InvalidOperationException(
                "Twilio configuration is incomplete. AccountSid, AuthToken and From are required.");
        }

        var websiteBaseUrl = configuration[AppSettings.Application.WebsiteBaseUrl];

        if (string.IsNullOrWhiteSpace(websiteBaseUrl))
        {
            throw new InvalidOperationException(
                "The public website base URL is not configured.");
        }

        SafeUrl safeUrl;

        try
        {
            safeUrl = SafeUrl.Create(
                websiteBaseUrl,
                requireHttps: true);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException(
                "The public website base URL must be a valid HTTPS URL.",
                ex);
        }

        var websiteUri = new Uri(safeUrl.Value, UriKind.Absolute);

        if (!string.IsNullOrEmpty(websiteUri.Query) ||
            !string.IsNullOrEmpty(websiteUri.Fragment) ||
            !string.IsNullOrEmpty(websiteUri.UserInfo))
        {
            throw new InvalidOperationException(
                "The public website base URL must not contain credentials, a query string or a fragment.");
        }

        _websiteBaseUrl = safeUrl.Value.TrimEnd('/');
    }

    /// <inheritdoc />
    public string BuildCelebrationInvitationUrl(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var invitationUrl =
            $"{_websiteBaseUrl}/celebrations/respond?token={Uri.EscapeDataString(token)}";

        return SafeUrl.Create(
            invitationUrl,
            requireHttps: true).Value;
    }

    /// <inheritdoc />
    public async Task<SmsSendResult> SendAsync(
        string toPhoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(toPhoneNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var client = new TwilioRestClient(
                _accountSid,
                _authToken);

            var response = await MessageResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(toPhoneNumber.Trim()),
                from: new Twilio.Types.PhoneNumber(_from.Trim()),
                body: message,
                client: client);

            if (response.ErrorCode.HasValue)
            {
                return new SmsSendResult(
                    IsSuccessful: false,
                    MessageId: response.Sid,
                    Status: response.Status?.ToString(),
                    ErrorMessage: response.ErrorMessage
                        ?? $"Twilio error code: {response.ErrorCode}");
            }

            return new SmsSendResult(
                IsSuccessful: true,
                MessageId: response.Sid,
                Status: response.Status?.ToString(),
                ErrorMessage: null);
        }
        catch (ApiException ex)
        {
            return new SmsSendResult(
                IsSuccessful: false,
                MessageId: null,
                Status: "Failed",
                ErrorMessage: $"Twilio API error {ex.Code}: {ex.Message}");
        }
    }
}
