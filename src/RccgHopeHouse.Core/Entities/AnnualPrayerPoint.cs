namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents an individual prayer point belonging
/// to an annual prayer.
/// </summary>
public class AnnualPrayerPoint : BaseEntity
{
    public Guid AnnualPrayerId { get; private set; }

    public string Text { get; private set; } =
        string.Empty;

    public int DisplayOrder { get; private set; }

    public AnnualPrayer AnnualPrayer { get; private set; } =
        null!;

    private AnnualPrayerPoint()
    {
    }

    public static AnnualPrayerPoint Create(
        Guid annualPrayerId,
        string text,
        int displayOrder)
    {
        if (annualPrayerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Annual prayer ID is required.",
                nameof(annualPrayerId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            text,
            nameof(text));

        if (displayOrder < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order must be at least 1.");
        }

        return new AnnualPrayerPoint
        {
            AnnualPrayerId = annualPrayerId,
            Text = text.Trim(),
            DisplayOrder = displayOrder
        };
    }

    public void Update(
        string text,
        int displayOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            text,
            nameof(text));

        if (displayOrder < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order must be at least 1.");
        }

        Text = text.Trim();
        DisplayOrder = displayOrder;

        MarkAsUpdated();
    }
}