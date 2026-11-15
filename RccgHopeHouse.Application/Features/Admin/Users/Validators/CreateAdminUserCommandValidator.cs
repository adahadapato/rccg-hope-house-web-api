using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates requests to create a new application user.
/// </summary>
public sealed class CreateAdminUserCommandValidator
    : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .EmailAddress()
            .WithMessage(ValidationMessages.InvalidEmail)
            .MaximumLength(256)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MinimumLength(8)
            .WithMessage(
                "{PropertyName} must be at least 8 characters long.")
            // These match the Identity password rules in
            // Infrastructure/DependencyInjection.cs. Checking them
            // here gives the admin a clear message instead of an
            // "unexpected error" when the account is created.
            .Matches("[A-Z]")
            .WithMessage(
                "{PropertyName} must contain at least one capital letter (A-Z).")
            .Matches("[a-z]")
            .WithMessage(
                "{PropertyName} must contain at least one small letter (a-z).")
            .Matches("[0-9]")
            .WithMessage(
                "{PropertyName} must contain at least one number (0-9).");

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage(ValidationMessages.Required);
    }
}
