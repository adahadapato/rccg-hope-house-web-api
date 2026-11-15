using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for the church information aggregate.
/// </summary>
public interface IChurchInfoRepository
{
    /// <summary>
    /// Gets the church information including its contact methods.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// The church information record, or null if none exists.
    /// </returns>
    Task<ChurchInfo?> GetAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new church information record.
    /// </summary>
    /// <param name="churchInfo">
    /// The church information to add.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(
        ChurchInfo churchInfo,
        CancellationToken ct = default);

    /// <summary>
    /// Marks an existing church information record for update.
    /// </summary>
    /// <param name="churchInfo">
    /// The church information to update.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    Task UpdateAsync(
        ChurchInfo churchInfo,
        CancellationToken ct = default);

    /// <summary>
    /// Explicitly adds a new contact method to the database context.
    /// </summary>
    /// <param name="contactMethod">
    /// The new church contact method.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    Task AddContactMethodAsync(
        ChurchContactMethod contactMethod,
        CancellationToken ct = default);

    /// <summary>
    /// Saves all pending changes.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of affected database records.</returns>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}