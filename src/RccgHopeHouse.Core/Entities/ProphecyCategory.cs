namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a category within a particular prophecy year.
///
/// Examples include General Prophecies, For Nigeria and
/// International Scene.
///
/// Categories are stored as entities rather than enums so that
/// administrators can change categories in future years without
/// requiring application code changes.
/// </summary>
public class ProphecyCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    /// <summary>
    /// Determines the order in which the category appears.
    /// Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Guid ProphecyYearId { get; private set; }

    public ProphecyYear ProphecyYear { get; private set; } = null!;

    public ICollection<Prophecy> Prophecies { get; private set; }
        = new List<Prophecy>();

    private ProphecyCategory()
    {
    }

    public static ProphecyCategory Create(
        Guid prophecyYearId,
        string name,
        int displayOrder,
        string? description = null)
    {
        if (prophecyYearId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid prophecy year is required.",
                nameof(prophecyYearId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            name,
            nameof(name));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        return new ProphecyCategory
        {
            ProphecyYearId = prophecyYearId,
            Name = name.Trim(),
            Description = NormalizeOptionalText(description),
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Update(
        string name,
        string? description,
        int displayOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            name,
            nameof(name));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Name = name.Trim();
        Description = NormalizeOptionalText(description);
        DisplayOrder = displayOrder;

        MarkAsUpdated();
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkAsUpdated();
    }

    public void ChangeDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        if (DisplayOrder == displayOrder)
        {
            return;
        }

        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}