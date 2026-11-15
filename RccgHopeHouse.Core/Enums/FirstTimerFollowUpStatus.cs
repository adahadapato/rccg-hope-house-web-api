namespace RccgHopeHouse.Core.Enums;

/// <summary>
/// Where the follow-up team has got to with a first-time visitor.
/// </summary>
public enum FirstTimerFollowUpStatus
{
    /// <summary>Nobody has been in touch yet.</summary>
    NotContacted = 0,

    /// <summary>Someone has called or messaged the visitor.</summary>
    Contacted = 1,

    /// <summary>The visitor has been visited in person.</summary>
    Visited = 2,

    /// <summary>No further follow-up is needed.</summary>
    Closed = 3
}
