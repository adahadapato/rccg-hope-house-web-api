using FluentValidation;
using RccgHopeHouse.Application.Features.Gallery.Commands;
using RccgHopeHouse.Core.Constants;

namespace RccgHopeHouse.Application.Features.Gallery.Validators;

public class UpdateGalleryImageCommandValidator : AbstractValidator<UpdateGalleryImageCommand>
{
    public UpdateGalleryImageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AltText).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Photographer).MaximumLength(100);
        RuleFor(x => x.NewContentType)
            .Must(ct => ct is null or "image/jpeg" or "image/png" or "image/webp" or "image/gif")
            .When(x => x.NewImageData is not null)
            .WithMessage(ValidationMessages.InvalidImageFormat);
    }
}