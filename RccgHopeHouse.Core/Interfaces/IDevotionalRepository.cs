using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for daily devotionals.
/// Provides data access for the public devotional experience
/// and admin management.
/// </summary>
public interface IDevotionalRepository
{
    /// <summary>
    /// Gets the most recent published devotional whose devotional
    /// date is today or earlier.
    ///
    /// Used by the public website. If today's devotional has not
    /// been published, the latest previously published devotional
    /// is returned instead.
    /// </summary>
    /// <param name="today">The current date used as the public visibility boundary.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<Devotional?> GetLatestPublishedAsync(
        DateOnly today,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a published devotional for a specific date.
    ///
    /// Used when a public visitor requests a past devotional.
    /// Application/API logic must prevent future dates from being
    /// requested publicly.
    /// </summary>
    /// <param name="devotionalDate">The devotional date to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<Devotional?> GetPublishedByDateAsync(
            DateOnly devotionalDate,
            DateOnly upToDate,
            CancellationToken ct = default);

    /// <summary>
    /// Gets published devotionals up to and including the supplied date.
    /// Used for public devotional history/browsing.
    /// Future devotionals are therefore excluded.
    /// </summary>
    /// <param name="upToDate">Maximum devotional date that may be returned.</param>
    /// <param name="skip">Number of devotionals to skip.</param>
    /// <param name="take">Maximum number of devotionals to return.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<Devotional>> GetPublishedHistoryAsync(
        DateOnly upToDate,
        int skip = 0,
        int take = 30,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all devotionals, including published, unpublished,
    /// past, current and future devotionals, for admin management.
    /// </summary>
    Task<IReadOnlyList<Devotional>> GetAllForAdminAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets a single devotional by ID for admin viewing/editing.
    /// </summary>
    Task<Devotional?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a devotional by its assigned devotional date
    /// for admin management.
    /// </summary>
    Task<Devotional?> GetByDateForAdminAsync(
        DateOnly devotionalDate,
        CancellationToken ct = default);

    /// <summary>
    /// Checks whether another devotional already exists for the
    /// specified date.
    ///
    /// excludeDevotionalId is used during editing so the current
    /// devotional does not conflict with itself.
    /// </summary>
    Task<bool> ExistsForDateAsync(
        DateOnly devotionalDate,
        Guid? excludeDevotionalId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new devotional to the database.
    /// </summary>
    Task AddAsync(
        Devotional devotional,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing devotional.
    /// </summary>
    Task UpdateAsync(
        Devotional devotional,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a devotional permanently.
    /// </summary>
    Task DeleteAsync(
        Devotional devotional,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the number of published devotionals up to and
    /// including the supplied date.
    /// Used for public history pagination.
    /// </summary>
    Task<int> GetPublishedCountAsync(
        DateOnly upToDate,
        CancellationToken ct = default);

    /// <summary>
    /// Persists all pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}