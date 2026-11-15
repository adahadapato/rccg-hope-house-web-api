using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Retrieves and processes Open Heavens devotional content
/// for a specified date.
/// </summary>
public interface IOpenHeavensDevotionalService
{
    /// <summary>
    /// Retrieves the devotional content for the requested date.
    /// Returns null when no suitable content is available.
    /// </summary>
    /// <param name="date">The devotional date.</param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    Task<OpenHeavensDevotionalResult?> GetDevotionalAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}