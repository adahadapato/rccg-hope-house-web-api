using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Entities;

public class ChurchEvent : BaseEntity
{
    /// <summary>
    /// Name of the event displayed on the website.
    /// </summary>
    public string Title { get; private set; } =
        string.Empty;

    /// <summary>
    /// Service category to which this event belongs.
    /// </summary>
    public ServiceCategory Category { get; private set; }

    /// <summary>
    /// Optional description of the event.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Date and time at which the event starts.
    /// </summary>
    public DateTime StartDateTime { get; private set; }

    /// <summary>
    /// Optional date and time at which the event ends.
    /// Supports both single-day and multi-day events.
    /// </summary>
    public DateTime? EndDateTime { get; private set; }

    /// <summary>
    /// Optional location or venue.
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Optional icon displayed on the public
    /// Upcoming Events card.
    /// Examples: 🎉, 👥, 💑
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Optional colour style used by the public
    /// Upcoming Events card.
    /// Examples: gold, blue, rose.
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Optional registration URL.
    /// </summary>
    public string? RegistrationUrl { get; private set; }

    /// <summary>
    /// Text displayed on the registration button.
    /// </summary>
    public string RegistrationButtonText { get; private set; } =
        "Register Now";

    /// <summary>
    /// Optional image associated with the event.
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Controls whether the event is available
    /// for public display.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Controls the display order of events.
    /// </summary>
    public int DisplayOrder { get; private set; }

    private ChurchEvent()
    {
    }

    public static ChurchEvent Create(
        string title,
        ServiceCategory category,
        DateTime startDateTime,
        DateTime? endDateTime = null,
        string? description = null,
        string? location = null,
        string? icon = null,
        string? color = null,
        string? registrationUrl = null,
        string? registrationButtonText = null,
        string? imageUrl = null,
        int displayOrder = 0,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        ValidateDates(
            startDateTime,
            endDateTime);

        ValidateDisplayOrder(
            displayOrder);

        return new ChurchEvent
        {
            Title = title.Trim(),
            Category = category,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Description =
                NormalizeOptionalText(
                    description),
            Location =
                NormalizeOptionalText(
                    location),
            Icon =
                NormalizeOptionalText(
                    icon),
            Color =
                NormalizeOptionalText(
                    color),
            RegistrationUrl =
                NormalizeOptionalText(
                    registrationUrl),
            RegistrationButtonText =
                string.IsNullOrWhiteSpace(
                    registrationButtonText)
                    ? "Register Now"
                    : registrationButtonText.Trim(),
            ImageUrl =
                NormalizeOptionalText(
                    imageUrl),
            DisplayOrder = displayOrder,
            IsActive = isActive
        };
    }

    public void Update(
        string title,
        ServiceCategory category,
        DateTime startDateTime,
        DateTime? endDateTime,
        string? description,
        string? location,
        string? icon,
        string? color,
        string? registrationUrl,
        string? registrationButtonText,
        string? imageUrl,
        int displayOrder,
        bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            title,
            nameof(title));

        ValidateDates(
            startDateTime,
            endDateTime);

        ValidateDisplayOrder(
            displayOrder);

        Title = title.Trim();
        Category = category;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;

        Description =
            NormalizeOptionalText(
                description);

        Location =
            NormalizeOptionalText(
                location);

        Icon =
            NormalizeOptionalText(
                icon);

        Color =
            NormalizeOptionalText(
                color);

        RegistrationUrl =
            NormalizeOptionalText(
                registrationUrl);

        RegistrationButtonText =
            string.IsNullOrWhiteSpace(
                registrationButtonText)
                ? "Register Now"
                : registrationButtonText.Trim();

        ImageUrl =
            NormalizeOptionalText(
                imageUrl);

        DisplayOrder = displayOrder;
        IsActive = isActive;

        MarkAsUpdated();
    }

    public void UpdateSchedule(
        DateTime startDateTime,
        DateTime? endDateTime)
    {
        ValidateDates(
            startDateTime,
            endDateTime);

        StartDateTime = startDateTime;
        EndDateTime = endDateTime;

        MarkAsUpdated();
    }

    public void SetRegistration(
        string? registrationUrl,
        string? registrationButtonText = null)
    {
        RegistrationUrl =
            NormalizeOptionalText(
                registrationUrl);

        RegistrationButtonText =
            string.IsNullOrWhiteSpace(
                registrationButtonText)
                ? "Register Now"
                : registrationButtonText.Trim();

        MarkAsUpdated();
    }

    public void SetDisplayOrder(
        int displayOrder)
    {
        ValidateDisplayOrder(
            displayOrder);

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

    public void ToggleActive()
    {
        IsActive = !IsActive;

        MarkAsUpdated();
    }

    private static void ValidateDates(
        DateTime startDateTime,
        DateTime? endDateTime)
    {
        if (
            endDateTime.HasValue &&
            endDateTime.Value <
            startDateTime)
        {
            throw new ArgumentException(
                "The event end date and time cannot be earlier than the start date and time.",
                nameof(endDateTime));
        }
    }

    private static void ValidateDisplayOrder(
        int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(
            value)
            ? null
            : value.Trim();
    }
}