using FluentValidation;
using RccgHopeHouse.Application.Features.ProphecyCategories.Commands;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Validators;

public class CreateProphecyCategoryCommandValidator
 : AbstractValidator<CreateProphecyCategoryCommand>
{
    public CreateProphecyCategoryCommandValidator()
    {
        RuleFor(x => x.ProphecyYearId)
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
