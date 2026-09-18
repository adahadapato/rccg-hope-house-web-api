using FluentValidation;
using RccgHopeHouse.Application.Features.Members.Commands;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.Members.Validators;

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.PhoneNumber)
            .Matches(PhoneNumber.Pattern)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.BirthMonth)
            .InclusiveBetween(1, 12)
            .When(x => x.BirthMonth.HasValue);

        RuleFor(x => x)
            .Must(x => x.BirthMonth.HasValue == x.BirthDay.HasValue)
            .WithMessage("Birth month and day must both be provided together, or both omitted.");

        RuleFor(x => x.WeddingAnniversary)
            .Must((cmd, anniversary) => !anniversary.HasValue || cmd.MaritalStatus == MaritalStatus.Married)
            .WithMessage("A wedding anniversary can only be set when marital status is Married.");
    }
}