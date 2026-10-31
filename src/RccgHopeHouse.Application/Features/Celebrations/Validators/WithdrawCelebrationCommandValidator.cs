
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates requests to withdraw celebration publication permission.
/// </summary>
public sealed class WithdrawCelebrationCommandValidator : AbstractValidator<WithdrawCelebrationCommand>
{
    /// <summary>
    /// Initializes validation rules for celebration withdrawal.
    /// </summary>
    public WithdrawCelebrationCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage("A valid celebration ID is required.");
    }
}
