
using FluentValidation;
using RccgHopeHouse.Application.Features.Celebrations.Commands;

namespace RccgHopeHouse.Application.Features.Celebrations.Validators;

/// <summary>
/// Validates incoming requests to create birthday
/// and wedding anniversary celebrations.
/// </summary>
/// <remarks>
/// Performs request-level validation before the command
/// reaches the application handler.
///
/// Database-dependent checks, such as member existence,
/// gallery category existence, and duplicate detection,
/// remain the responsibility of the command handler.
///
/// Celebration dates are evaluated using the UK local
/// calendar date rather than the server's UTC date.
/// </remarks>
public sealed class CreateCelebrationCommandValidator
    : AbstractValidator<CreateCelebrationCommand>
{
    /// <summary>
    /// Initializes the validation rules for creating
    /// a celebration.
    /// </summary>
    public CreateCelebrationCommandValidator()
    {
        // Member identifier is required.
        RuleFor(x => x.MemberId)
            .NotEmpty()
            .WithMessage(
                "A valid member ID is required.");

        // Gallery category identifier is required.
        RuleFor(x => x.GalleryCategoryId)
            .NotEmpty()
            .WithMessage(
                "A valid gallery category ID is required.");

        // Only supported occasions are accepted.
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

        // Celebration date cannot be in the past.
        RuleFor(x => x.CelebrationDate)
            .Must(date => date >= GetCurrentUkDate())
            .When(x => x.CelebrationDate != default)
            .WithMessage(
                "The celebration date cannot be in the past.");
    }

    /// <summary>
    /// Retrieves the current calendar date in the
    /// United Kingdom, accounting for GMT and BST.
    /// </summary>
    /// <returns>
    /// The current UK local date.
    /// </returns>
    /// <remarks>
    /// Supports both Windows and Linux hosting environments
    /// by resolving the appropriate time zone identifier.
    /// </remarks>
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
