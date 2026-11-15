using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Users.Validators;

/// <summary>
/// Validates requests to update the roles assigned
/// to an application user.
/// </summary>
public sealed class UpdateAdminUserRolesCommandValidator
    : AbstractValidator<UpdateAdminUserRolesCommand>
{
    public UpdateAdminUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage(ValidationMessages.Required);

        RuleForEach(x => x.Roles)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required.Replace("{PropertyName}", "Role name"))
            .MaximumLength(256)
            .WithMessage(ValidationMessages.MaxLength.Replace("{PropertyName}", "Role name").Replace("{MaxLength}", "256"));
    }
}