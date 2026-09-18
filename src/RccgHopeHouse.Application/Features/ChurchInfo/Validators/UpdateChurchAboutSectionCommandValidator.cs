using FluentValidation;
using RccgHopeHouse.Application.Features.ChurchInfo.Commands;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Validators;

public class UpdateChurchAboutSectionCommandValidator : AbstractValidator<UpdateChurchAboutSectionCommand>
{
    public UpdateChurchAboutSectionCommandValidator()
    {
        RuleFor(x => x.ParishName)
            .NotEmpty().WithMessage("Parish name is required.")
            .MaximumLength(200);

        RuleFor(x => x.EstablishedYear)
            .InclusiveBetween(1900, DateTime.UtcNow.Year)
            .WithMessage("Established year must be a realistic past year.");

        RuleFor(x => x.Tagline)
            .NotEmpty().WithMessage("Tagline is required.")
            .MaximumLength(300);

        RuleFor(x => x.AboutLead)
            .NotEmpty().WithMessage("About lead text is required.")
            .MaximumLength(500);

        RuleFor(x => x.AboutText)
            .NotEmpty().WithMessage("About text is required.")
            .MaximumLength(2000);

        RuleFor(x => x.MultiCulturalStat)
            .NotEmpty().WithMessage("Multi-cultural stat is required.")
            .MaximumLength(20);
    }
}