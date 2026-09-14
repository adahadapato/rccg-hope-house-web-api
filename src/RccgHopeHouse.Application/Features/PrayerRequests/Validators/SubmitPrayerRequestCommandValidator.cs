using FluentValidation;
using RccgHopeHouse.Application.Features.PrayerRequests.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Validators;

public class SubmitPrayerRequestCommandValidator : AbstractValidator<SubmitPrayerRequestCommand>
{
    public SubmitPrayerRequestCommandValidator()
    {
        RuleFor(x => x.RequesterName)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(100).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(2000).WithMessage("Prayer request cannot exceed 2,000 characters.");

        RuleFor(x => x.RequesterEmail)
            .EmailAddress().WithMessage(ValidationMessages.InvalidEmail)
            .When(x => !string.IsNullOrWhiteSpace(x.RequesterEmail));

        RuleFor(x => x.PhoneNumber)
            .Must(phone => string.IsNullOrWhiteSpace(phone))
            .WithMessage("Invalid phone number format. Use 10-15 digits with optional +, spaces, dashes, or parentheses.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}