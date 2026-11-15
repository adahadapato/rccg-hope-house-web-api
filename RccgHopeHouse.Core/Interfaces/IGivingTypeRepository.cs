using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces
{
    /// <summary>
    /// Defines persistence operations for giving types.
    /// </summary>
    public interface IGivingTypeRepository
    {
        /// <summary>
        /// Gets a giving type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the giving type.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The giving type if found; otherwise, null.
        /// </returns>
        Task<GivingType?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        /// <summary>
        /// Gets all giving types, including inactive ones,
        /// ordered by their display order.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A read-only list of all giving types.
        /// </returns>
        Task<IReadOnlyList<GivingType>> GetAllAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Gets all active giving types that can currently be
        /// displayed on the public Give Online form.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// A read-only list of active giving types.
        /// </returns>
        Task<IReadOnlyList<GivingType>> GetActiveAsync(
            CancellationToken ct = default);

        /// <summary>
        /// Determines whether a giving type with the specified
        /// name already exists.
        /// </summary>
        /// <param name="name">The giving type name to check.</param>
        /// <param name="excludeId">
        /// Optional giving type identifier to exclude from the check.
        /// This is useful when updating an existing giving type.
        /// </param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// True if another giving type with the specified name exists;
        /// otherwise, false.
        /// </returns>
        Task<bool> ExistsByNameAsync(
            string name,
            Guid? excludeId = null,
            CancellationToken ct = default);

        /// <summary>
        /// Adds a new giving type.
        /// </summary>
        /// <param name="givingType">The giving type to add.</param>
        /// <param name="ct">Cancellation token.</param>
        Task AddAsync(
            GivingType givingType,
            CancellationToken ct = default);

        /// <summary>
        /// Updates an existing giving type.
        /// </summary>
        /// <param name="givingType">The giving type to update.</param>
        /// <param name="ct">Cancellation token.</param>
        Task UpdateAsync(
            GivingType givingType,
            CancellationToken ct = default);

        /// <summary>
        /// Saves all pending changes to the data store.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// The number of affected records.
        /// </returns>
        Task<int> SaveChangesAsync(
            CancellationToken ct = default);
    }
}