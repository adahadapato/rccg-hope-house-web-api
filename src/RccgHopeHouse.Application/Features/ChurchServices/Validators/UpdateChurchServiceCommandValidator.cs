using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchServices.Validators;

/// <summary>
/// Validates UpdateChurchServiceCommand.
///
/// Midnight-crossing services are valid, so StartTime is deliberately
/// not required to be earlier than EndTime.
/// </summary>
public class UpdateChurchServiceCommandValidator
    : AbstractValidator<UpdateChurchServiceCommand>
{
    public UpdateChurchServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.ZoomId)
            .NotEmpty()
            .When(x =>
                x.Location?.Equals(
                    "Zoom",
                    StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage(
                "Zoom ID is required when location is Zoom.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Display order must be 0 or greater.");

        RuleFor(x => x.Icon)
            .MaximumLength(50)
            .When(x =>
                !string.IsNullOrWhiteSpace(x.Icon))
            .WithMessage(
                "Icon must not exceed 50 characters.");

        RuleFor(x => x.DayOfMonth)
            .InclusiveBetween(1, 31)
            .When(x => x.DayOfMonth.HasValue)
            .WithMessage(
                "Day of month must be between 1 and 31.");
    }
}