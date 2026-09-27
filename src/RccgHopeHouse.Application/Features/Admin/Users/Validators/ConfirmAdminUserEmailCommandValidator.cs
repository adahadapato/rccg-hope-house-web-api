using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates an email confirmation request.
/// </summary>
public sealed class ConfirmAdminUserEmailCommandValidator
    : AbstractValidator<ConfirmAdminUserEmailCommand>
{
    public ConfirmAdminUserEmailCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(command => command.Token)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);
    }
}