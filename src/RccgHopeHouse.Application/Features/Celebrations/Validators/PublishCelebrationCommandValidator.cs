
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates requests to publish a celebration.
/// </summary>
public sealed class PublishCelebrationCommandValidator : AbstractValidator<PublishCelebrationCommand>
{
    /// <summary>
    /// Initializes validation rules for celebration publication.
    /// </summary>
    public PublishCelebrationCommandValidator()
    {
        RuleFor(x => x.CelebrationId)
            .NotEmpty()
            .WithMessage("A valid celebration ID is required.");
    }
}
