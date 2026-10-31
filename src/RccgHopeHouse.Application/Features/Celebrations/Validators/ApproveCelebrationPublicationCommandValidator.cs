
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates a member's request to approve
/// publication of their celebration.
/// </summary>
public sealed class ApproveCelebrationPublicationCommandValidator
    : AbstractValidator<ApproveCelebrationPublicationCommand>
{
    /// <summary>
    /// Initializes validation rules for publication approval.
    /// </summary>
    public ApproveCelebrationPublicationCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage(
                "A valid celebration ID is required.");
    }
}
