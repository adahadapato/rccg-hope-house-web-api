using FluentValidation;
using RccgHopeHouse.Application.Features.Devotionals.Commands;

namespace RccgHopeHouse.Application.Features.Devotionals.Validators;

/// <summary>
/// Validates UnpublishDevotionalCommand before handler execution.
/// </summary>
public class UnpublishDevotionalCommandValidator
    : AbstractValidator<UnpublishDevotionalCommand>
{
    public UnpublishDevotionalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Devotional ID is required.");
    }
}