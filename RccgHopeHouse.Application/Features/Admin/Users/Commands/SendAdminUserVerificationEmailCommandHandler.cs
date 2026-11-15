using MediatR;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.Admin;


using System.Net;


/// <summary>
/// Handles <see cref="SendAdminUserVerificationEmailCommand"/>.
/// </summary>
public sealed class SendAdminUserVerificationEmailCommandHandler
    : IRequestHandler<
        SendAdminUserVerificationEmailCommand,
        EmailResult>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IEmailService _emailService;

    public SendAdminUserVerificationEmailCommandHandler(
        IAdminRepository adminRepository,
        IEmailService emailService)
    {
        _adminRepository = adminRepository;
        _emailService = emailService;
    }

    public async Task<EmailResult> Handle(SendAdminUserVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user =   await _adminRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException($"User with ID '{request.UserId}' was not found.");
        }

        if (user.EmailConfirmed)
        {
            throw new InvalidOperationException( "This user's email address is already confirmed.");
        }

        var token =
            await _adminRepository
                .GenerateEmailConfirmationTokenAsync(
                    user.Id,
                    cancellationToken);

        var verificationBaseUrl =
            request.VerificationBaseUrl.TrimEnd('/');

        var verificationUrl =
            $"{verificationBaseUrl}" +
            $"?userId={Uri.EscapeDataString(user.Id)}" +
            $"&token={Uri.EscapeDataString(token)}";

        var displayName =
            string.Join(
                " ",
                new[]
                {
                    user.FirstName,
                    user.LastName
                }
                .Where(name =>
                    !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim()));

        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = user.Email;
        }

        var safeDisplayName =
            WebUtility.HtmlEncode(displayName);

        var safeVerificationUrl =
            WebUtility.HtmlEncode(verificationUrl);

        var htmlBody = $"""
            <p>Hello {safeDisplayName},</p>

            <p>
                An account has been created for you on the
                RCCG Hope House administration system.
            </p>

            <p>
                Please verify your email address by clicking
                the button below.
            </p>

            <p>
                <a
                    href="{safeVerificationUrl}"
                    style="
                        display:inline-block;
                        padding:12px 20px;
                        background:#333333;
                        color:#ffffff;
                        text-decoration:none;
                        border-radius:4px;">
                    Verify Email Address
                </a>
            </p>

            <p>
                If the button does not work, copy and paste
                the following address into your browser:
            </p>

            <p>
                {safeVerificationUrl}
            </p>

            <p>
                If you were not expecting this email,
                please contact RCCG Hope House.
            </p>

            <p>
                RCCG Hope House
            </p>
            """;

        return await _emailService.SendAsync(user.Email, "Verify your RCCG Hope House email address", htmlBody, cancellationToken);
    }
}