using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates requests to activate or deactivate
/// an application user.
/// </summary>
public sealed class SetAdminUserStatusCommandValidator
    : AbstractValidator<SetAdminUserStatusCommand>
{
    public SetAdminUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);
    }
}