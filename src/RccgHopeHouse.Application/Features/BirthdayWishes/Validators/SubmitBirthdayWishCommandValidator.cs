using FluentValidation;
using RccgHopeHouse.Application.Features.BirthdayWishes.Commands;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.BirthdayWishes.Validators;

public class SubmitBirthdayWishCommandValidator
    : AbstractValidator<SubmitBirthdayWishCommand>
{
    public SubmitBirthdayWishCommandValidator()
    {
        RuleFor(x => x.GalleryImageId)
            .NotEmpty().WithMessage("Please choose a celebrant photo.");

        RuleFor(x => x.AuthorName)
            .NotEmpty().WithName("Your name").WithMessage(ValidationMessages.Required)
            .MinimumLength(2).WithMessage("Please enter your name.")
            .MaximumLength(GalleryImageComment.AuthorNameMaxLength)
            .WithName("Your name").WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Message)
            .NotEmpty().WithName("Your wish").WithMessage(ValidationMessages.Required)
            .MinimumLength(2).WithMessage("Please write a short birthday wish.")
            .MaximumLength(GalleryImageComment.MessageMaxLength)
            .WithName("Your wish").WithMessage(ValidationMessages.MaxLength);
    }
}
