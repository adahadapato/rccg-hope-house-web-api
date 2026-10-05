using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for service broadcasts.
/// Supports public retrieval of published broadcasts for
/// broadcast-enabled church services, category-based history,
/// synchronization lookups, publication management,
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
    /// Gets a service broadcast by its YouTube video identifier.
    /// </summary>
    /// <param name="videoId">
    /// The unique YouTube video identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The matching service broadcast, or <c>null</c>
    /// when no broadcast exists for the supplied video.
    /// </returns>
    Task<ServiceBroadcast?> GetByVideoIdAsync(
        string videoId,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the broadcast for a specific church service
    /// and service month.
    /// </summary>
    /// <param name="churchServiceId">
    /// The church service identifier.
    /// </param>
    /// <param name="serviceMonth">
    /// A date within the required service month.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The matching broadcast, or <c>null</c> when no
    /// broadcast exists for that service and month.
    /// </returns>
    Task<ServiceBroadcast?> GetByServiceAndMonthAsync(
        Guid churchServiceId,
        DateTime serviceMonth,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all broadcasts belonging to a specific church service.
    /// This is used when managing publication state so that an older
    /// broadcast can be unpublished when a newer broadcast becomes
    /// the current published broadcast.
    /// </summary>
    /// <param name="churchServiceId">
    /// The church service identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Broadcasts belonging to the supplied church service,
    /// ordered from newest to oldest.
    /// </returns>
    Task<IReadOnlyList<ServiceBroadcast>>
        GetByChurchServiceIdAsync(
            Guid churchServiceId,
            CancellationToken ct = default);

    /// <summary>
    /// Gets the current/latest published broadcast for a category.
    /// Retained for existing category-based public functionality.
    /// </summary>
    /// <param name="category">
    /// The service category to search.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The latest published broadcast for the supplied category,
    /// or <c>null</c> when no published broadcast exists.
    /// </returns>
    Task<ServiceBroadcast?> GetLatestByCategoryAsync(
        ServiceCategory category,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the latest published broadcast for each active church
    /// service that has broadcasting enabled.
    /// </summary>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The latest published broadcast for each eligible church service.
    /// </returns>
    Task<IReadOnlyList<ServiceBroadcast>>
        GetLatestForBroadcastEnabledServicesAsync(
            CancellationToken ct = default);

    /// <summary>
    /// Gets published broadcast history for a category.
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
    /// Published broadcasts belonging to the supplied category,
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
    /// including unpublished broadcasts, ordered from newest
    /// to oldest.
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
    /// Adds a new service broadcast.
    /// </summary>
    /// <param name="broadcast">
    /// Broadcast to add.
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
    /// Broadcast to update.
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
    /// Broadcast to delete.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task DeleteAsync(
        ServiceBroadcast broadcast,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending repository changes.
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