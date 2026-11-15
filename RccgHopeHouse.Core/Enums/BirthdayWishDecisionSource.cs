namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Who made the latest decision about a birthday wish.
/// </summary>
public enum BirthdayWishDecisionSource
{
    /// <summary>The automatic AI check.</summary>
    Ai = 1,

    /// <summary>An administrator on the Birthday Wishes page.</summary>
    Admin = 2,

    /// <summary>
    /// A built-in rule, e.g. the AI check was unavailable,
    /// the message contained a link, or "hold everything" is on.
    /// </summary>
    System = 3
}
