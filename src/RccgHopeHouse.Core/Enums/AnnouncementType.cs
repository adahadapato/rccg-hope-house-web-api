namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Kind of News &amp; Updates item. Drives the badge colour
/// and label shown on the public floating banner.
/// </summary>
public enum AnnouncementType
{
    /// <summary>General church news.</summary>
    News = 1,

    /// <summary>Promotion of an upcoming programme or event.</summary>
    Event = 2,

    /// <summary>Practical information, e.g. service time changes.</summary>
    Notice = 3,

    /// <summary>Time-critical information, e.g. a cancellation.</summary>
    Urgent = 4
}
