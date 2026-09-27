using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Roles.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Validators;

/// <summary>
/// Validates requests to update an existing application role.
/// </summary>
public sealed class UpdateAdminRoleCommandValidator
    : AbstractValidator<UpdateAdminRoleCommand>
{
    public UpdateAdminRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(256)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(ValidationMessages.MaxLength);
    }
}