using FluentValidation;
using RccgHopeHouse.Application.Features.Offerings.Commands;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Offerings.Validators;

/// <summary>
/// Validates requests to create an offering.
/// </summary>
public class CreateOfferingCommandValidator
    : AbstractValidator<CreateOfferingCommand>
{
    public CreateOfferingCommandValidator()
    {
        RuleFor(x => x.GivingTypeId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200)
            .When(x => !x.IsAnonymous);

        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => !x.IsAnonymous);

        RuleFor(x => x.Email)
            .Must(email =>
            {
                if (string.IsNullOrWhiteSpace(email))
                    return true;

                try
                {
                    EmailAddress.Create(email);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            })
            .WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.MessageReference)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.MessageReference));
    }
}