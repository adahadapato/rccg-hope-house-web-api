using FluentValidation;
using RccgHopeHouse.Application.Features.ProphecyYears.Commands;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Validators;

public class CreateProphecyYearCommandValidator
: AbstractValidator<CreateProphecyYearCommand>
{
    public CreateProphecyYearCommandValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 9999);
    }
}
