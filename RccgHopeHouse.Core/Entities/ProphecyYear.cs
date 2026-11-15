namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a year for which RCCG prophecies are stored.
///
/// Years are database entities rather than hard-coded values so that
/// future prophecy years can be added without application code changes.
/// </summary>
public class ProphecyYear : BaseEntity
{
    public int Year { get; private set; }

    /// <summary>
    /// Determines whether this prophecy year is available publicly.
    /// </summary>
    public bool IsPublished { get; private set; }

    /// <summary>
    /// Categories belonging to this prophecy year.
    /// </summary>
    public ICollection<ProphecyCategory> Categories { get; private set; }
        = new List<ProphecyCategory>();

    private ProphecyYear()
    {
    }

    public static ProphecyYear Create(
        int year,
        bool isPublished = false)
    {
        ValidateYear(year);

        return new ProphecyYear
        {
            Year = year,
            IsPublished = isPublished
        };
    }

    public void ChangeYear(int year)
    {
        ValidateYear(year);

        if (Year == year)
        {
            return;
        }

        Year = year;
        MarkAsUpdated();
    }

    public void Publish()
    {
        if (IsPublished)
        {
            return;
        }

        IsPublished = true;
        MarkAsUpdated();
    }

    public void Unpublish()
    {
        if (!IsPublished)
        {
            return;
        }

        IsPublished = false;
        MarkAsUpdated();
    }

    private static void ValidateYear(int year)
    {
        if (year < 1900 || year > 9999)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                "Prophecy year must be between 1900 and 9999.");
        }
    }
}