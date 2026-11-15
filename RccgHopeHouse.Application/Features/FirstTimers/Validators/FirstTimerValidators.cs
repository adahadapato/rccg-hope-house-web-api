using FluentValidation;
using RccgHopeHouse.Application.Features.FirstTimers.Commands;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Application.Features.FirstTimers.Validators;

public class SubmitFirstTimerCommandValidator
    : AbstractValidator<SubmitFirstTimerCommand>
{
    public SubmitFirstTimerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(FirstTimer.NameMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(FirstTimer.NameMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Matches(PhoneNumber.Pattern)
            .WithMessage("Invalid phone number format. Use 10-15 digits with optional +, spaces, dashes, or parentheses.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(ValidationMessages.InvalidEmail)
            .MaximumLength(255).WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Please give an email address if you would like to be contacted by email.")
            .When(x => x.PreferredContactMethod == FirstTimerContactMethod.Email);

        RuleFor(x => x.PreferredContactMethod)
            .IsInEnum().WithMessage("Please choose how you would like to be contacted.");

        RuleFor(x => x.Source)
            .IsInEnum().WithMessage("Please choose how you heard about us.")
            .When(x => x.Source.HasValue);

        RuleFor(x => x.ConsentToContact)
            .Equal(true)
            .WithMessage("Please tick the box to agree to us storing your details.");

        // The address is optional, but a half-filled address is not useful.
        When(HasAnyAddressPart, () =>
        {
            RuleFor(x => x.AddressLine1)
                .NotEmpty().WithMessage("Please enter the first line of your address.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Please enter your town or city.");

            RuleFor(x => x.Postcode)
                .NotEmpty().WithMessage("Please enter your postcode.");
        });

        RuleFor(x => x.AddressLine1).MaximumLength(200).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.AddressLine2).MaximumLength(200).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.City).MaximumLength(100).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.Postcode).MaximumLength(20).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x)
            .Must(HaveValidBirthday)
            .WithName("Birthday")
            .WithMessage("Please give both the day and the month of your birthday.");

        RuleFor(x => x.InvitedBy)
            .MaximumLength(FirstTimer.InvitedByMaxLength).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.PrayerRequest)
            .MaximumLength(2000).WithMessage("Prayer request cannot exceed 2,000 characters.");

        RuleFor(x => x.Comment)
            .MaximumLength(FirstTimer.CommentMaxLength).WithMessage(ValidationMessages.MaxLength);
    }

    private static bool HasAnyAddressPart(SubmitFirstTimerCommand command)
    {
        return
            !string.IsNullOrWhiteSpace(command.AddressLine1) ||
            !string.IsNullOrWhiteSpace(command.AddressLine2) ||
            !string.IsNullOrWhiteSpace(command.City) ||
            !string.IsNullOrWhiteSpace(command.Postcode);
    }

    private static bool HaveValidBirthday(SubmitFirstTimerCommand command)
    {
        if (!command.BirthMonth.HasValue && !command.BirthDay.HasValue)
        {
            return true;
        }

        if (!command.BirthMonth.HasValue || !command.BirthDay.HasValue)
        {
            return false;
        }

        try
        {
            Birthday.Create(command.BirthMonth.Value, command.BirthDay.Value);

            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}

public class UpdateFirstTimerFollowUpCommandValidator
    : AbstractValidator<UpdateFirstTimerFollowUpCommand>
{
    public UpdateFirstTimerFollowUpCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid follow-up status.");

        RuleFor(x => x.Note)
            .MaximumLength(FirstTimer.FollowUpNoteMaxLength).WithMessage(ValidationMessages.MaxLength);
    }
}
