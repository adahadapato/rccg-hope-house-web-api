using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchInfo.Commands;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Validators;

public class UpdateChurchAddressCommandValidator : AbstractValidator<UpdateChurchAddressCommand>
{
    public UpdateChurchAddressCommandValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address line 1 is required.")
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100);

        RuleFor(x => x.PostCode)
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100);
    }
}