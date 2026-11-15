
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates a member's request to decline
/// celebration publication.
/// </summary>
public sealed class DisapproveCelebrationCommandValidator
    : AbstractValidator<DisapproveCelebrationCommand>
{
    /// <summary>
    /// Initializes validation rules for celebration disapproval.
    /// </summary>
    public DisapproveCelebrationCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage(
                "A valid celebration ID is required.");
    }
}
