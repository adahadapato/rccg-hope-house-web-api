using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for annual prayer content.
/// </summary>
public interface IAnnualPrayerRepository
{
    /// <summary>
    /// Gets the annual prayer for a specific year,
    /// including its prayer points.
    /// </summary>
    Task<AnnualPrayer?> GetByYearAsync(
        int year,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the active annual prayer that should
    /// currently be displayed on the public website.
    /// </summary>
    Task<AnnualPrayer?> GetActiveAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets the most recent annual prayer by year,
    /// including its prayer points.
    /// </summary>
    Task<AnnualPrayer?> GetLatestAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets all annual prayers, most recent year first,
    /// for the administration interface.
    /// </summary>
    Task<IReadOnlyList<AnnualPrayer>> GetAllAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets an annual prayer by its identifier,
    /// including its prayer points.
    /// </summary>
    Task<AnnualPrayer?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new annual prayer.
    /// </summary>
    Task AddAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing annual prayer.
    /// </summary>
    Task UpdateAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes an annual prayer.
    /// </summary>
    Task DeleteAsync(
        AnnualPrayer annualPrayer,
        CancellationToken ct = default);

    /// <summary>
    /// Removes prayer points belonging to an
    /// annual prayer.
    /// </summary>
    Task RemovePrayerPointsAsync(
        IEnumerable<AnnualPrayerPoint> prayerPoints,
        CancellationToken ct = default);

    /// <summary>
    /// Deactivates all active annual prayers except
    /// the specified annual prayer.
    /// </summary>
    Task DeactivateOtherActiveAsync(
        Guid annualPrayerId,
        CancellationToken ct = default);

    /// <summary>
    /// Saves pending changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}