using FluentValidation;
using RccgHopeHouse.Application.Features.Gallery.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Gallery.Validators;

public class UploadGalleryImageCommandValidator : AbstractValidator<UploadGalleryImageCommand>
{
    public UploadGalleryImageCommandValidator()
    {
        RuleFor(x => x.ImageData)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(data => data.Length <= 10 * 1024 * 1024)
            .WithMessage(ValidationMessages.FileTooLarge.Replace("{MaxSize}", "10"));

        RuleFor(x => x.ContentType)
            .Must(ct => ct is "image/jpeg" or "image/png" or "image/webp" or "image/gif")
            .WithMessage(ValidationMessages.InvalidImageFormat);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(200).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.AltText)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .MaximumLength(255).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Photographer)
            .MaximumLength(100).WithMessage(ValidationMessages.MaxLength);
    }
}