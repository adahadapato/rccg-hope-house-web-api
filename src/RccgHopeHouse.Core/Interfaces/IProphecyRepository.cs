using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository contract for managing individual prophecy statements
/// and retrieving complete published prophecy content.
/// </summary>
public interface IProphecyRepository
{
    /// <summary>
    /// Gets an individual prophecy by its unique identifier.
    /// The returned entity is tracked for command operations.
    /// </summary>
    Task<Prophecy?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets prophecy statements belonging to a category.
    /// </summary>
    Task<IReadOnlyList<Prophecy>> GetByCategoryAsync(
        Guid categoryId,
        bool activeOnly = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a complete prophecy year including its categories
    /// and prophecy statements.
    /// </summary>
    Task<ProphecyYear?> GetCompleteYearAsync(
        int year,
        bool publishedOnly = true,
        bool activeOnly = true,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the latest published prophecy year including its
    /// active categories and active prophecy statements.
    /// </summary>
    Task<ProphecyYear?> GetLatestCompletePublishedYearAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new prophecy statement.
    /// </summary>
    Task AddAsync(
        Prophecy prophecy,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy statement for update.
    /// </summary>
    Task UpdateAsync(
        Prophecy prophecy,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy statement for deletion.
    /// </summary>
    Task DeleteAsync(
        Prophecy prophecy,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}