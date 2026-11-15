namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Categories for organizing Pastor's Corner posts.
/// Used for filtering and visual organization in the scrolling feed.
/// </summary>
public enum PostCategory
{
    /// <summary>
    /// Welcome messages and greetings.
    /// Example: "Welcome to 2026", "New Member Welcome"
    /// </summary>
    Welcome = 1,

    /// <summary>
    /// Church announcements and important notices.
    /// Example: "Service Time Changes", "Special Events"
    /// </summary>
    Announcement = 2,

    /// <summary>
    /// Devotional messages and spiritual reflections.
    /// Example: Daily devotionals, spiritual encouragement
    /// </summary>
    Devotional = 3,

    /// <summary>
    /// Prophetic words and declarations.
    /// Example: "Prophecy for 2026", "Monthly Prophetic Word"
    /// </summary>
    Prophetic = 4,

    /// <summary>
    /// Event-specific posts.
    /// Example: "Easter Service", "Conference Announcement"
    /// </summary>
    Event = 5,

    /// <summary>
    /// General pastoral messages and teachings.
    /// Example: General encouragement, teachings
    /// </summary>
    General = 6,

    /// <summary>
    /// Prayer points and intercession guides.
    /// Example: "Monthly Prayer Points", "Prayer Guide"
    /// </summary>
    Prayer = 7,

    /// <summary>
    /// Testimonies and success stories.
    /// Example: "Testimony Sunday Highlights"
    /// </summary>
    Testimony = 8
}