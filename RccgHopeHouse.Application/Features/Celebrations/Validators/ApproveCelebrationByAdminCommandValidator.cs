
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates administrator approval requests for celebrations.
/// </summary>
public sealed class ApproveCelebrationByAdminCommandValidator : AbstractValidator<ApproveCelebrationByAdminCommand>
{
    /// <summary>
    /// Initializes validation rules for administrator approval.
    /// </summary>
    public ApproveCelebrationByAdminCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage("A valid celebration ID is required.");
    }
}
