namespace RccgHopeHouse.Core.ValueObjects;

/// <summary>
/// Represents a member's birthday as an immutable value object.
/// Month and day are required when a birthday is provided,
/// while the birth year is optional.
/// </summary>
public sealed record Birthday
{
    public int Month { get; private init; }
    public int Day { get; private init; }
    public int? Year { get; private init; }

    private Birthday()
    {
    }

    /// <summary>
    /// Creates a validated birthday.
    /// </summary>
    /// <param name="month">The birth month, from 1 to 12.</param>
    /// <param name="day">The birth day of the month.</param>
    /// <param name="year">
    /// The optional birth year. A member may provide their month and day
    /// without providing their year.
    /// </param>
    /// <returns>A validated <see cref="Birthday"/> value object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the month, day, or year is outside the permitted range.
    /// </exception>
    public static Birthday Create(
        int month,
        int day,
        int? year = null)
    {
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(month),
                "Birth month must be between 1 and 12.");
        }

        if (year.HasValue &&
            (year.Value < 1900 || year.Value > DateTime.UtcNow.Year))
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                "Birth year must be a realistic past year.");
        }

        // When the year is unknown, use a leap year so that
        // 29 February remains a valid month/day birthday.
        var validationYear = year ?? 2000;

        var daysInMonth = DateTime.DaysInMonth(
            validationYear,
            month);

        if (day < 1 || day > daysInMonth)
        {
            throw new ArgumentOutOfRangeException(
                nameof(day),
                "Birth day is not valid for the given month and year.");
        }

        return new Birthday
        {
            Month = month,
            Day = day,
            Year = year
        };
    }
}