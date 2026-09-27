using FluentValidation;
using RccgHopeHouse.Application.Features.Admin.Roles.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Admin.Roles.Validators;

/// <summary>
/// Validates requests to create a new application role.
/// </summary>
public sealed class CreateAdminRoleCommandValidator
    : AbstractValidator<CreateAdminRoleCommand>
{
    public CreateAdminRoleCommandValidator()
    {
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