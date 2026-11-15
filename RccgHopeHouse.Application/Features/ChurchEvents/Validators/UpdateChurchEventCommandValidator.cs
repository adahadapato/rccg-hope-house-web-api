using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchEvents.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Validators;

public class UpdateChurchEventCommandValidator
    : AbstractValidator<UpdateChurchEventCommand>
{
    public UpdateChurchEventCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(
                "Event ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(150)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage(
                "Invalid service category.");

        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .WithMessage(
                "Start date and time is required.");

        RuleFor(x => x.EndDateTime)
            .GreaterThanOrEqualTo(
                x => x.StartDateTime)
            .When(
                x => x.EndDateTime.HasValue)
            .WithMessage(
                "End date and time cannot be earlier than the start date and time.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Description))
            .WithMessage(
                "Description must not exceed 1000 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(200)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Location))
            .WithMessage(
                "Location must not exceed 200 characters.");

        RuleFor(x => x.Icon)
            .MaximumLength(50)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Icon))
            .WithMessage(
                "Icon must not exceed 50 characters.");

        RuleFor(x => x.Color)
            .MaximumLength(50)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.Color))
            .WithMessage(
                "Color must not exceed 50 characters.");

        RuleFor(x => x.RegistrationUrl)
            .MaximumLength(1000)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.RegistrationUrl))
            .WithMessage(
                "Registration URL must not exceed 1000 characters.");

        RuleFor(x => x.RegistrationButtonText)
            .MaximumLength(100)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.RegistrationButtonText))
            .WithMessage(
                "Registration button text must not exceed 100 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000)
            .When(
                x => !string.IsNullOrWhiteSpace(
                    x.ImageUrl))
            .WithMessage(
                "Image URL must not exceed 1000 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Display order must be 0 or greater.");
    }
}