namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents an individual prophecy statement within a
/// prophecy category.
/// </summary>
public class Prophecy : BaseEntity
{
    /// <summary>
    /// The full text of the prophecy.
    /// </summary>
    public string Text { get; private set; } = string.Empty;

    public Guid CategoryId { get; private set; }

    public ProphecyCategory Category { get; private set; } = null!;

    /// <summary>
    /// Determines the order in which the prophecy appears
    /// within its category.
    /// Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Prophecy()
    {
    }

    public static Prophecy Create(
        Guid categoryId,
        string text,
        int displayOrder)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid prophecy category is required.",
                nameof(categoryId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            text,
            nameof(text));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        return new Prophecy
        {
            CategoryId = categoryId,
            Text = text.Trim(),
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Update(
        string text,
        int displayOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            text,
            nameof(text));

        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        Text = text.Trim();
        DisplayOrder = displayOrder;

        MarkAsUpdated();
    }

    public void ChangeCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid prophecy category is required.",
                nameof(categoryId));
        }

        if (CategoryId == categoryId)
        {
            return;
        }

        CategoryId = categoryId;
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
}