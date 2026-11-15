using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchServices.Validators;

/// <summary>
/// Validates CreateChurchServiceCommand before handler execution.
///
/// Midnight-crossing services are valid, so StartTime is deliberately
/// not required to be earlier than EndTime.
/// </summary>
public class CreateChurchServiceCommandValidator
    : AbstractValidator<CreateChurchServiceCommand>
{
    public CreateChurchServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid service category.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum()
            .WithMessage("Invalid day of week.");

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