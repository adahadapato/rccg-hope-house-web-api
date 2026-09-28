using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchEvents.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Validators;

/// <summary>
/// Validates CreateChurchEventCommand
/// before handler execution.
/// </summary>
public class CreateChurchEventCommandValidator
    : AbstractValidator<CreateChurchEventCommand>
{
    public CreateChurchEventCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(150)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid service category.");

        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .WithMessage("Start date and time is required.");

        RuleFor(x => x.EndDateTime)
            .GreaterThanOrEqualTo(x => x.StartDateTime)
            .When(x => x.EndDateTime.HasValue)
            .WithMessage("End date and time cannot be earlier than the start date and time.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Location)
            .MaximumLength(200)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Location))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Icon)
            .MaximumLength(50)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Icon))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Color)
            .MaximumLength(50)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Color))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.RegistrationUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.RegistrationUrl))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.RegistrationButtonText)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.RegistrationButtonText))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be 0 or greater.");
    }
}