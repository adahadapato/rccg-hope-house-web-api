using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates requests to send an email verification
/// message to an administrative user.
/// </summary>
public sealed class SendAdminUserVerificationEmailCommandValidator
    : AbstractValidator<SendAdminUserVerificationEmailCommand>
{
    public SendAdminUserVerificationEmailCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(command => command.VerificationBaseUrl)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .Must(BeValidAbsoluteHttpUrl)
            .WithMessage(ValidationMessages.InvalidUrl);
    }

    private static bool BeValidAbsoluteHttpUrl(string url)
    {
        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttps ||
               uri.Scheme == Uri.UriSchemeHttp;
    }
}