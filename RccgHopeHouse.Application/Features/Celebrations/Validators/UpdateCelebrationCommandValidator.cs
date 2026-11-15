
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates requests to update existing celebrations.
/// </summary>
/// <remarks>
/// Performs request-level validation of celebration
/// identifiers, gallery categories, occasions and dates.
///
/// Database-dependent checks are handled by
/// UpdateCelebrationCommandHandler.
///
/// Dates are evaluated using the United Kingdom
/// calendar date, accounting for GMT and BST.
/// </remarks>
public sealed class UpdateCelebrationCommandValidator
    : AbstractValidator<UpdateCelebrationCommand>
{
    /// <summary>
    /// Initializes the validation rules for updating
    /// an existing celebration.
    /// </summary>
    public UpdateCelebrationCommandValidator()
    {
        // Celebration identifier is required.
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(
                "A valid celebration ID is required.");

        // Gallery category identifier is required.
        RuleFor(x => x.GalleryCategoryId)
            .NotEmpty()
            .WithMessage(
                "A valid gallery category ID is required.");

        // Validate the celebration occasion.
        RuleFor(x => x.Occasion)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                "A celebration occasion is required.")
            .Must(occasion =>
                occasion is "Birthday" or "WeddingAnniversary")
            .WithMessage(
                "Occasion must be Birthday or WeddingAnniversary.");

        // Celebration date must be supplied.
        RuleFor(x => x.CelebrationDate)
            .NotEmpty()
            .WithMessage(
                "A valid celebration date is required.");

        // Prevent scheduling celebrations in the past.
        RuleFor(x => x.CelebrationDate)
            .Must(date => date >= GetCurrentUkDate())
            .When(x => x.CelebrationDate != default)
            .WithMessage(
                "The celebration date cannot be in the past.");
    }

    /// <summary>
    /// Gets the current UK local calendar date,
    /// accounting for GMT and British Summer Time.
    /// </summary>
    /// <returns>
    /// The current date in the United Kingdom.
    /// </returns>
    private static DateOnly GetCurrentUkDate()
    {
        TimeZoneInfo ukTimeZone;

        try
        {
            ukTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                "Europe/London");
        }
        catch (TimeZoneNotFoundException)
        {
            ukTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                "GMT Standard Time");
        }

        var ukNow = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ukTimeZone);

        return DateOnly.FromDateTime(ukNow);
    }
}
