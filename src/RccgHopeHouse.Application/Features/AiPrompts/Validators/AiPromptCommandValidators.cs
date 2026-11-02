using FluentValidation;
using RccgHopeHouse.Application.Features.AiPrompts.Commands;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.AiPrompts.Validators;

public class SaveAiPromptCommandValidator
    : AbstractValidator<SaveAiPromptCommand>
{
    public SaveAiPromptCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Instructions)
            .NotEmpty().WithName("Prompt").WithMessage(ValidationMessages.Required)
            .MaximumLength(AiPromptVersion.InstructionsMaxLength)
            .WithName("Prompt").WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.ChangeNote)
            .MaximumLength(AiPromptVersion.ChangeNoteMaxLength)
            .WithName("Change note").WithMessage(ValidationMessages.MaxLength);
    }
}

public class SuggestAiPromptCommandValidator
    : AbstractValidator<SuggestAiPromptCommand>
{
    public const int RequestedChangeMaxLength = 1000;

    public SuggestAiPromptCommandValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage(ValidationMessages.Required);

        RuleFor(x => x.CurrentInstructions)
            .MaximumLength(AiPromptVersion.InstructionsMaxLength)
            .WithName("Prompt").WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.RequestedChange)
            .NotEmpty().WithName("What to change").WithMessage(ValidationMessages.Required)
            .MaximumLength(RequestedChangeMaxLength)
            .WithName("What to change").WithMessage(ValidationMessages.MaxLength);
    }
}
