using FluentValidation;
using RccgHopeHouse.Application.Features.ProphecyYears.Commands;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Validators;

public class UpdateProphecyYearCommandValidator
 : AbstractValidator<UpdateProphecyYearCommand>
{
    public UpdateProphecyYearCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 9999);
    }
}
