namespace RccgHopeHouse.Core.Results.AI;

/// <summary>
/// Describes the outcome of an Open Heavens devotional
/// synchronisation attempt.
/// </summary>
/// <param name="DevotionalDate">
/// The date requested for synchronisation.
/// </param>
/// <param name="Status">
/// The outcome: Created, Published, AlreadyPublished,
/// AlreadyExists, NotFound, or InvalidContent.
/// </param>
/// <param name="Message">
/// A human-readable explanation of the outcome.
/// </param>
public sealed record OpenHeavensDevotionalSynchronizationResult(
    DateOnly DevotionalDate,
    string Status,
    string Message);