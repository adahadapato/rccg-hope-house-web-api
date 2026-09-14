using FluentValidation;
using RccgHopeHouse.Application.Features.PrayerRequests.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Validators;

public class UpdatePrayerRequestStatusCommandValidator : AbstractValidator<UpdatePrayerRequestStatusCommand>
{
    public UpdatePrayerRequestStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.NewStatus).IsInEnum().WithMessage("Invalid prayer request status.");
        RuleFor(x => x.PastoralNote).MaximumLength(1000).WithMessage(ValidationMessages.MaxLength);
    }
}