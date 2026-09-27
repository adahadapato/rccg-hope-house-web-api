using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Roles.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Validators;

/// <summary>
/// Validates requests to delete an application role.
/// </summary>
public sealed class DeleteAdminRoleCommandValidator
    : AbstractValidator<DeleteAdminRoleCommand>
{
    public DeleteAdminRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);
    }
}