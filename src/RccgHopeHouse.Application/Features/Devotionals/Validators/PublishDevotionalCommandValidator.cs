using FluentValidation;
using RccgHopeHouse.Application.Features.Devotionals.Commands;

namespace RccgHopeHouse.Application.Features.Devotionals.Validators;

/// <summary>
/// Validates PublishDevotionalCommand before handler execution.
/// </summary>
public class PublishDevotionalCommandValidator
    : AbstractValidator<PublishDevotionalCommand>
{
    public PublishDevotionalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Devotional ID is required.");
    }
}