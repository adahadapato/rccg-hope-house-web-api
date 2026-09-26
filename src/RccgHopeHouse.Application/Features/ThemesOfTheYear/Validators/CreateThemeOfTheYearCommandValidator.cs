using FluentValidation;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Commands;

namespace RccgHopeHouse.Application.Features.ThemesOfTheYear.Validators;

public class CreateThemeOfTheYearCommandValidator
    : AbstractValidator<CreateThemeOfTheYearCommand>
{
    public CreateThemeOfTheYearCommandValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.ThemeTitle)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ScriptureText)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.ScriptureReference)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PrimaryDescription)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.SecondaryDescription)
            .MaximumLength(2000)
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.SecondaryDescription));

        RuleFor(x => x.CallToActionText)
            .MaximumLength(300)
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.CallToActionText));
    }
}