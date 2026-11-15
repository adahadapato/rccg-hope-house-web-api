using FluentValidation;
using RccgHopeHouse.Application.Features.Prophecies.Commands;

namespace RccgHopeHouse.Application.Features.Prophecies.Validators;

public class CreateProphecyCommandValidator
    : AbstractValidator<CreateProphecyCommand>
{
    public CreateProphecyCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}