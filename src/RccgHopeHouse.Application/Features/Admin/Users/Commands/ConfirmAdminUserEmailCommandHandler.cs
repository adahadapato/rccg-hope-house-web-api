using MediatR;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.Admin;
using System.Net;

namespace RccgHopeHouse.Application.Features.Admin.Users.Commands;

/// <summary>
/// Handles <see cref="ConfirmAdminUserEmailCommand"/>.
/// </summary>
public sealed class ConfirmAdminUserEmailCommandHandler
    : IRequestHandler<ConfirmAdminUserEmailCommand>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IEmailService _emailService;

    public ConfirmAdminUserEmailCommandHandler(
        IAdminRepository adminRepository,
        IEmailService emailService)
    {
        _adminRepository = adminRepository;
        _emailService = emailService;
    }

    public async Task Handle(
        ConfirmAdminUserEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _adminRepository.GetUserByIdAsync(
                request.UserId,
                cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with ID '{request.UserId}' was not found.");
        }

        if (user.EmailConfirmed)
        {
            return;
        }

        await _adminRepository.ConfirmEmailAsync(
            request.UserId,
            request.Token,
            cancellationToken);

        var displayName =
            string.Join(
                " ",
                new[] { user.FirstName, user.LastName }
                    .Where(name =>
                        !string.IsNullOrWhiteSpace(name))
                    .Select(name => name!.Trim()));

        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = user.Email;
        }

        var safeDisplayName =
            WebUtility.HtmlEncode(displayName);

        var htmlBody = $"""
            <p>Hello {safeDisplayName},</p>

            <p>
                Your email address has been successfully verified
                and your RCCG Hope House account is now active.
            </p>

            <p>
                You may now proceed to log in to your account.
            </p>

            <p>
                If you did not complete this verification or believe
                this was done in error, please contact the RCCG Hope
                House team.
            </p>

            <p>
                Kind regards,<br />
                <strong>RCCG Hope House</strong>
            </p>
            """;

        await _emailService.SendAsync(
            user.Email,
            "Your RCCG Hope House email has been verified",
            htmlBody,
            cancellationToken);
    }
}