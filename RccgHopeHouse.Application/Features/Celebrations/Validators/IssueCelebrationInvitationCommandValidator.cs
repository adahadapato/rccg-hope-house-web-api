
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates requests to issue celebration invitations.
/// </summary>
public sealed class IssueCelebrationInvitationCommandValidator : AbstractValidator<IssueCelebrationInvitationCommand>
{
    /// <summary>
    /// Initialises the invitation command validation rules.
    /// </summary>
    public IssueCelebrationInvitationCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage("A valid celebration ID is required.");
    }
}
