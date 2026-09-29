using FluentValidation;
using RccgHopeHouse.Application.Features.Devotionals.Commands;

namespace RccgHopeHouse.Application.Features.Devotionals.Validators;

/// <summary>
/// Validates UpdateDevotionalCommand.
/// Applies devotional content validation plus ID validation.
/// </summary>
public class UpdateDevotionalCommandValidator
    : AbstractValidator<UpdateDevotionalCommand>
{
    public UpdateDevotionalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Devotional ID is required.");

        RuleFor(x => x.DevotionalDate)
            .NotEmpty()
            .WithMessage("Devotional date is required.");

        RuleFor(x => x.Theme)
            .NotEmpty()
            .WithMessage("Theme is required.")
            .MaximumLength(200)
            .WithMessage("Theme cannot exceed 200 characters.");

        RuleFor(x => x.ScriptureReference)
            .NotEmpty()
            .WithMessage("Scripture reference is required.")
            .MaximumLength(150)
            .WithMessage("Scripture reference cannot exceed 150 characters.");

        RuleFor(x => x.PassageId)
            .NotEmpty()
            .WithMessage("Passage ID is required.")
            .MaximumLength(150)
            .WithMessage("Passage ID cannot exceed 150 characters.");

        RuleFor(x => x.Thought)
            .NotEmpty()
            .WithMessage("Thought is required.")
            .MaximumLength(2000)
            .WithMessage("Thought cannot exceed 2,000 characters.");

        RuleFor(x => x.CommentaryPoints)
            .NotNull()
            .WithMessage("Commentary points are required.")
            .NotEmpty()
            .WithMessage("At least one commentary point is required.");

        RuleForEach(x => x.CommentaryPoints)
            .NotEmpty()
            .WithMessage("Commentary points cannot be empty.");

        RuleFor(x => x.PrayerPoints)
            .NotNull()
            .WithMessage("Prayer points are required.")
            .NotEmpty()
            .WithMessage("At least one prayer point is required.");

        RuleForEach(x => x.PrayerPoints)
            .NotEmpty()
            .WithMessage("Prayer points cannot be empty.");

        RuleFor(x => x.Declaration)
            .NotEmpty()
            .WithMessage("Declaration is required.")
            .MaximumLength(2000)
            .WithMessage("Declaration cannot exceed 2,000 characters.");
    }
}