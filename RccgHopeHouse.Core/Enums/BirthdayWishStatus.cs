namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Where a birthday wish is in the moderation process.
/// Only <see cref="Approved"/> wishes are shown on the website.
/// </summary>
public enum BirthdayWishStatus
{
    /// <summary>Hidden from visitors until an admin approves it.</summary>
    Pending = 1,

    /// <summary>Visible under the celebrant's photo.</summary>
    Approved = 2,

    /// <summary>Never shown on the website.</summary>
    Rejected = 3
}
