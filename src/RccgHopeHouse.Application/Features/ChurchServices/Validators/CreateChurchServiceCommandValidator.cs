using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchServices.Validators;

/// <summary>
/// Validates CreateChurchServiceCommand before handler execution.
/// Ensures location rules and required fields are correct.
/// NOTE: the previous StartTime &lt;= EndTime rule was removed — several
/// real services (Last Friday Vigil, Holy Ghost Service) legitimately
/// cross midnight (e.g. 22:00-01:00), and a naive time comparison can't
/// safely distinguish that from a genuine data-entry error. Trusting the
/// admin entering the schedule rather than rejecting valid late-night
/// services.
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

        // Zoom fields required if location is Zoom
        RuleFor(x => x.ZoomId)
            .NotEmpty().When(x => x.Location?.Equals("Zoom", System.StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage("Zoom ID is required when location is Zoom.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
    }
}