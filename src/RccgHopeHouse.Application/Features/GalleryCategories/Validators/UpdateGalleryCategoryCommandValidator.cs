using FluentValidation;
using RccgHopeHouse.Application.Features.GalleryCategories.Commands;

namespace RccgHopeHouse.Application.Features.GalleryCategories.Validators;

public class UpdateGalleryCategoryCommandValidator
    : AbstractValidator<UpdateGalleryCategoryCommand>
{
    public UpdateGalleryCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}