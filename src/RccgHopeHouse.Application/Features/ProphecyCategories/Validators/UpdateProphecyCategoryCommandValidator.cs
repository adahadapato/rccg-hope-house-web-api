using FluentValidation;
using RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Validators;

public class UpdateProphecyCategoryCommandValidator
: AbstractValidator<UpdateProphecyCategoryCommand>
{
    public UpdateProphecyCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(
                x =>
                    !string.IsNullOrWhiteSpace(
                        x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
