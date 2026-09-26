using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository contract for managing prophecy categories.
/// </summary>
public interface IProphecyCategoryRepository
{
    /// <summary>
    /// Gets a prophecy category by its unique identifier.
    /// The returned entity is tracked for command operations.
    /// </summary>
    Task<ProphecyCategory?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the prophecy categories belonging to a prophecy year.
    /// </summary>
    Task<IReadOnlyList<ProphecyCategory>> GetByYearAsync(
        Guid prophecyYearId,
        bool activeOnly = false,
        CancellationToken ct = default);

    /// <summary>
    /// Determines whether a category name already exists
    /// within the specified prophecy year.
    /// </summary>
    Task<bool> ExistsByNameAsync(
        Guid prophecyYearId,
        string name,
        Guid? excludeId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new prophecy category.
    /// </summary>
    Task AddAsync(
        ProphecyCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy category for update.
    /// </summary>
    Task UpdateAsync(
        ProphecyCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a prophecy category for deletion.
    /// </summary>
    Task DeleteAsync(
        ProphecyCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}