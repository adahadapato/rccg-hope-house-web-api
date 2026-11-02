using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Checks a visitor's birthday wish before it is shown
/// on the website.
/// </summary>
public interface ICommentModerationService
{
    /// <summary>
    /// True when the "hold everything" setting is on
    /// (BirthdayWishes:ModerationMode = HoldAll). Every wish
    /// then waits for an admin and the AI is not called.
    /// </summary>
    bool HoldAllForReview { get; }

    /// <summary>
    /// Returns Approve, Reject or Review for the wish.
    /// Throws when the AI provider is unavailable; callers
    /// must then hold the wish for an admin.
    /// </summary>
    Task<CommentModerationResult> ModerateAsync(
        string authorName,
        string message,
        CancellationToken cancellationToken = default);
}
