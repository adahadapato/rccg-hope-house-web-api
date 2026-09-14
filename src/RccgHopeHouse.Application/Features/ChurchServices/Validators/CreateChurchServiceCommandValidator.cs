using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchServices.Validators;

/// <summary>
/// Validates CreateChurchServiceCommand before handler execution.
/// Ensures time logic, location rules, and required fields are correct.
/// </summary>
public class CreateChurchServiceCommandValidator : AbstractValidator<CreateChurchServiceCommand>
{
    public CreateChurchServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(100).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid service category.");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Invalid day of week.");

        // Validate time logic: if both provided, start must be before end
        RuleFor(x => x)
            .Must(x => !x.StartTime.HasValue || !x.EndTime.HasValue || x.StartTime.Value <= x.EndTime.Value)
            .WithMessage("Start time must be before end time.");

        // Zoom fields required if location is Zoom
        RuleFor(x => x.ZoomId)
            .NotEmpty().When(x => x.Location?.Equals("Zoom", System.StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage("Zoom ID is required when location is Zoom.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
    }
}