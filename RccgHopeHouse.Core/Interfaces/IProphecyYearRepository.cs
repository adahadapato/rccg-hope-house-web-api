using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository contract for managing prophecy years.
/// </summary>
public interface IProphecyYearRepository
{
    /// <summary>
    /// Gets a prophecy year by its unique identifier.
    /// The returned entity is tracked for command operations.
    /// </summary>
    Task<ProphecyYear?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a prophecy year by its calendar year.
    /// </summary>
    Task<ProphecyYear?> GetByYearAsync(
        int year,
        bool publishedOnly = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all prophecy years.
    /// </summary>
    Task<IReadOnlyList<ProphecyYear>> GetAllAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets all published prophecy years.
    /// </summary>
    Task<IReadOnlyList<ProphecyYear>> GetPublishedAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Gets the most recent published prophecy year.
    /// </summary>
    Task<ProphecyYear?> GetLatestPublishedAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Determines whether a prophecy year already exists.
    /// </summary>
    Task<bool> ExistsAsync(
        int year,
        Guid? excludeId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new prophecy year.
    /// </summary>
    Task AddAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy year for update.
    /// </summary>
    Task UpdateAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy year for deletion.
    /// </summary>
    Task DeleteAsync(
        ProphecyYear prophecyYear,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}