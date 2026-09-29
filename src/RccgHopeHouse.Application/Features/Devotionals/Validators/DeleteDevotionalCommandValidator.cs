using FluentValidation;
using RccgHopeHouse.Application.Features.Devotionals.Commands;

namespace RccgHopeHouse.Application.Features.Devotionals.Validators;

/// <summary>
/// Validates DeleteDevotionalCommand before handler execution.
/// </summary>
public class DeleteDevotionalCommandValidator
    : AbstractValidator<DeleteDevotionalCommand>
{
    public DeleteDevotionalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Devotional ID is required.");
    }
}