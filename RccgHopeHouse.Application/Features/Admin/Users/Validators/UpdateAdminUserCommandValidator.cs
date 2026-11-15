using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates requests to update an existing application user.
/// </summary>
public sealed class UpdateAdminUserCommandValidator
    : AbstractValidator<UpdateAdminUserCommand>
{
    public UpdateAdminUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

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
    }
}