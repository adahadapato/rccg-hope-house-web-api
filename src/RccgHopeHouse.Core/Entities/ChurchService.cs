namespace RccgHopeHouse.Core.Entities;

public class ChurchService : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public ServiceCategory Category { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeSpan? StartTime { get; private set; }
    public TimeSpan? EndTime { get; private set; }
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public string? ZoomId { get; private set; }
    public string? ZoomPasscode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }

    public RecurrencePattern Recurrence { get; private set; }
        = RecurrencePattern.Weekly;

    public int? DayOfMonth { get; private set; }
    public string? AdditionalInfo { get; private set; }

    /// <summary>
    /// True for services held by this local parish; false for services
    /// broadcast/streamed from RCCG HQ.
    /// </summary>
    public bool IsLocal { get; private set; } = true;

    /// <summary>
    /// Optional icon displayed for the service on the website.
    /// This removes the need to hard-code icons against service names
    /// in the frontend.
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Controls whether this service appears in the public
    /// Special Monthly Services section.
    ///
    /// This is deliberately separate from IsActive:
    /// IsActive controls whether the service itself is active,
    /// while ShowInMonthlyServices controls placement in that
    /// particular public website section.
    /// </summary>
    public bool ShowInMonthlyServices { get; private set; }

    /// <summary>
    /// Controls whether video broadcasts may be associated with
    /// this church service.
    ///
    /// This is deliberately independent of the service category
    /// and its public website placement. A service may therefore
    /// support broadcasts without requiring its category to be
    /// hard-coded into the broadcast system.
    /// </summary>
    public bool IsBroadcastEnabled { get; private set; }

    /// <summary>
    /// Theme for the upcoming or currently occurring instance
    /// of this church service.
    ///
    /// This is deliberately stored on the service rather than on
    /// a broadcast because a theme may be known before a YouTube
    /// livestream or recording is available.
    ///
    /// Historical broadcast themes remain stored on their
    /// corresponding ServiceBroadcast records.
    /// </summary>
    public string? CurrentTheme { get; private set; }

    /// <summary>
    /// Monthly broadcasts associated directly with this service.
    /// </summary>
    public ICollection<ServiceBroadcast> Broadcasts { get; private set; }
        = new List<ServiceBroadcast>();

    private ChurchService()
    {
    }

    public static ChurchService Create(
        string name,
        ServiceCategory category,
        DayOfWeek dayOfWeek,
        TimeSpan? startTime,
        TimeSpan? endTime,
        string? description = null,
        RecurrencePattern recurrence = RecurrencePattern.Weekly,
        int? dayOfMonth = null,
        bool isLocal = true,
        string? icon = null,
        bool showInMonthlyServices = false,
        bool isBroadcastEnabled = false,
        string? currentTheme = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            name,
            nameof(name));

        return new ChurchService
        {
            Name = name.Trim(),
            Category = category,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            Description = description?.Trim(),
            Recurrence = recurrence,
            DayOfMonth = dayOfMonth,
            IsLocal = isLocal,
            Icon = NormalizeOptionalText(icon),
            ShowInMonthlyServices = showInMonthlyServices,
            IsBroadcastEnabled = isBroadcastEnabled,
            CurrentTheme = NormalizeOptionalText(currentTheme),
            DisplayOrder = 0,
            IsActive = true
        };
    }

    public void UpdateSchedule(
        TimeSpan? startTime,
        TimeSpan? endTime,
        string? location,
        string? zoomId,
        string? zoomPasscode)
    {
        StartTime = startTime;
        EndTime = endTime;
        Location = NormalizeOptionalText(location);
        ZoomId = NormalizeOptionalText(zoomId);
        ZoomPasscode = NormalizeOptionalText(zoomPasscode);

        MarkAsUpdated();
    }

    public void Update(
        string name,
        ServiceCategory category,
        DayOfWeek dayOfWeek,
        TimeSpan? startTime,
        TimeSpan? endTime,
        string? description,
        RecurrencePattern recurrence,
        int? dayOfMonth,
        bool isLocal,
        string? icon = null,
        bool showInMonthlyServices = false,
        bool isBroadcastEnabled = false,
        string? currentTheme = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            name,
            nameof(name));

        Name = name.Trim();
        Category = category;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        Description = NormalizeOptionalText(description);
        Recurrence = recurrence;
        DayOfMonth = dayOfMonth;
        IsLocal = isLocal;
        Icon = NormalizeOptionalText(icon);
        ShowInMonthlyServices = showInMonthlyServices;
        IsBroadcastEnabled = isBroadcastEnabled;
        CurrentTheme = NormalizeOptionalText(currentTheme);

        MarkAsUpdated();
    }

    public void ToggleActive()
    {
        IsActive = !IsActive;
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

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayOrder),
                "Display order cannot be negative.");
        }

        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }

    public void SetMonthlyServicesDisplay(
        bool showInMonthlyServices,
        string? icon)
    {
        ShowInMonthlyServices = showInMonthlyServices;
        Icon = NormalizeOptionalText(icon);

        MarkAsUpdated();
    }

    /// <summary>
    /// Enables or disables video broadcasts for this service.
    /// </summary>
    public void SetBroadcastEnabled(bool isBroadcastEnabled)
    {
        if (IsBroadcastEnabled == isBroadcastEnabled)
        {
            return;
        }

        IsBroadcastEnabled = isBroadcastEnabled;
        MarkAsUpdated();
    }

    /// <summary>
    /// Sets or clears the theme for the upcoming or currently
    /// occurring instance of this service.
    /// </summary>
    /// <param name="currentTheme">
    /// The current service theme, or null/empty to clear it.
    /// </param>
    public void SetCurrentTheme(string? currentTheme)
    {
        var normalizedTheme =
            NormalizeOptionalText(currentTheme);

        if (CurrentTheme == normalizedTheme)
        {
            return;
        }

        CurrentTheme = normalizedTheme;
        MarkAsUpdated();
    }

    public void SetAdditionalInfo(string? additionalInfo)
    {
        AdditionalInfo =
            NormalizeOptionalText(additionalInfo);

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