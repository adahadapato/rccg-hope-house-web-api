using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchServices.Validators;

/// <summary>
/// Validates UpdateChurchServiceCommand.
/// Reuses create rules plus ID validation.
/// NOTE: the previous StartTime &lt;= EndTime rule was removed — see
/// CreateChurchServiceCommandValidator for the reasoning (midnight-crossing
/// services like Last Friday Vigil and Holy Ghost Service would have been
/// incorrectly rejected).
/// </summary>
public class UpdateChurchServiceCommandValidator : AbstractValidator<UpdateChurchServiceCommand>
{
    public UpdateChurchServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(100);

        RuleFor(x => x.Category)
            .IsInEnum();

        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.ZoomId)
            .NotEmpty().When(x => x.Location?.Equals("Zoom", System.StringComparison.OrdinalIgnoreCase) == true)
            .WithMessage("Zoom ID is required when location is Zoom.");
    }
}