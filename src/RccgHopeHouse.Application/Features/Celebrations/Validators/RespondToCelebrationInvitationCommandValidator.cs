
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates the request to respond to a celebration invitation.
/// </summary>
public sealed class RespondToCelebrationInvitationCommandValidator : AbstractValidator<RespondToCelebrationInvitationCommand>
{
    /// <summary>
    /// Configures validation rules for invitation responses.
    /// </summary>
    public RespondToCelebrationInvitationCommandValidator()
    {
        RuleFor(x => x.Token)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The invitation token is required.")
            .Length(64)
            .WithMessage("The invitation token is invalid.")
            .Matches("^[A-Fa-f0-9]{64}$")
            .WithMessage("The invitation token is invalid.");
    }
}
