using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for service broadcasts.
/// Supports public retrieval of the latest broadcasts for
/// broadcast-enabled church services, category-based history,
/// and administrative CRUD operations.
/// </summary>
public interface IServiceBroadcastRepository
{
    /// <summary>
    /// Gets a service broadcast by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the service broadcast.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The matching service broadcast, or <c>null</c>
    /// when no broadcast is found.
    /// </returns>
    Task<ServiceBroadcast?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the current/latest broadcast for a category.
    /// Retained for existing category-based functionality.
    /// </summary>
    /// <param name="category">
    /// The service category to search.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The latest broadcast for the supplied category,
    /// or <c>null</c> when no broadcast exists.
    /// </returns>
    Task<ServiceBroadcast?> GetLatestByCategoryAsync(
        ServiceCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the latest broadcast for each active church service
    /// that has broadcasting enabled.
    /// </summary>
    /// <remarks>
    /// Broadcast eligibility is determined by the related
    /// <see cref="ChurchService.IsBroadcastEnabled"/> property,
    /// rather than by a hard-coded list of service categories.
    ///
    /// <see cref="ServiceBroadcast.ChurchServiceId"/> is the
    /// authoritative relationship used to group broadcasts
    /// by church service.
    /// </remarks>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A collection containing the latest broadcast for each
    /// active, broadcast-enabled church service that has at
    /// least one broadcast.
    /// </returns>
    Task<IReadOnlyList<ServiceBroadcast>>
        GetLatestForBroadcastEnabledServicesAsync(
            CancellationToken ct = default);

    /// <summary>
    /// Gets broadcast history for a category.
    /// Retained for existing category-based functionality.
    /// </summary>
    /// <param name="category">
    /// The service category to search.
    /// </param>
    /// <param name="skip">
    /// The number of records to skip.
    /// </param>
    /// <param name="take">
    /// The maximum number of records to return.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Broadcasts belonging to the supplied category,
    /// ordered from newest to oldest.
    /// </returns>
    Task<IReadOnlyList<ServiceBroadcast>>
        GetAllByCategoryAsync(
            ServiceCategory category,
            int skip,
            int take,
            CancellationToken ct = default);

    /// <summary>
    /// Gets all service broadcasts for administration,
    /// ordered from newest to oldest.
    /// </summary>
    /// <param name="skip">
    /// The number of records to skip.
    /// </param>
    /// <param name="take">
    /// The maximum number of records to return.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A paged collection of service broadcasts.
    /// </returns>
    Task<IReadOnlyList<ServiceBroadcast>>
        GetAllAsync(
            int skip,
            int take,
            CancellationToken ct = default);

    /// <summary>
    /// Adds a new service broadcast to the repository.
    /// </summary>
    /// <param name="broadcast">
    /// The service broadcast to add.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task AddAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default);

    /// <summary>
    /// Marks an existing service broadcast for update.
    /// </summary>
    /// <param name="broadcast">
    /// The service broadcast to update.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task UpdateAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a service broadcast for deletion.
    /// </summary>
    /// <param name="broadcast">
    /// The service broadcast to delete.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task DeleteAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending repository changes to the database.
    /// </summary>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}