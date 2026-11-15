namespace RccgHopeHouse.Core.Results.AI;

/// <summary>What the automatic check decided about a wish.</summary>
public enum CommentModerationVerdict
{
    /// <summary>A genuine, kind wish: publish it.</summary>
    Approve = 1,

    /// <summary>Unacceptable or spam: never publish it.</summary>
    Reject = 2,

    /// <summary>Not sure: hold it for an admin.</summary>
    Review = 3
}

/// <summary>
/// Result of the automatic birthday wish check.
/// </summary>
/// <param name="Verdict">Approve, Reject or Review.</param>
/// <param name="Reason">Short explanation shown to admins.</param>
public sealed record CommentModerationResult(
    CommentModerationVerdict Verdict,
    string Reason);
