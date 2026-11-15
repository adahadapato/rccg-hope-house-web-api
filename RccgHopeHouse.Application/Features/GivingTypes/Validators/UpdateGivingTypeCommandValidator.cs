using FluentValidation;
using RccgHopeHouse.Application.Features.GivingTypes.Commands;

namespace RccgHopeHouse.Application.Features.GivingTypes.Validators;

/// <summary>
/// Validates requests to update an existing giving type.
/// </summary>
public class UpdateGivingTypeCommandValidator
    : AbstractValidator<UpdateGivingTypeCommand>
{
    public UpdateGivingTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}