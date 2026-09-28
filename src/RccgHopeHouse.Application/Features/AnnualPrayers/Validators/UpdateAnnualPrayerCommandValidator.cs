using FluentValidation;
using RccgHopeHouse.Application.Features.AnnualPrayers.Commands;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Validators;

public class UpdateAnnualPrayerCommandValidator
    : AbstractValidator<UpdateAnnualPrayerCommand>
{
    public UpdateAnnualPrayerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(x => x.Theme)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Service)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BibleReference)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BibleText)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Declaration)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.ClosingVerse)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000)
            .When(x =>
                !string.IsNullOrWhiteSpace(
                    x.ImageUrl));

        RuleFor(x => x.PrayerPoints)
            .NotNull()
            .NotEmpty()
            .WithMessage(
                "At least one prayer point is required.");

        RuleForEach(x => x.PrayerPoints)
            .ChildRules(point =>
            {
                point.RuleFor(x => x.Text)
                    .NotEmpty()
                    .MaximumLength(2000);

                point.RuleFor(x =>
                        x.DisplayOrder)
                    .GreaterThan(0);
            });

        RuleFor(x => x.PrayerPoints)
            .Must(HaveUniqueDisplayOrders)
            .When(x =>
                x.PrayerPoints is not null)
            .WithMessage(
                "Prayer point display orders must be unique.");
    }

    private static bool HaveUniqueDisplayOrders(
        IReadOnlyList<UpdateAnnualPrayerPointRequest>
            prayerPoints)
    {
        return prayerPoints
            .Select(point =>
                point.DisplayOrder)
            .Distinct()
            .Count() ==
            prayerPoints.Count;
    }
}