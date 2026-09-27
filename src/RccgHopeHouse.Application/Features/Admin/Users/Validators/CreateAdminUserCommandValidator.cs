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
                "{PropertyName} must be at least 8 characters long.");

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage(ValidationMessages.Required);
    }
}
