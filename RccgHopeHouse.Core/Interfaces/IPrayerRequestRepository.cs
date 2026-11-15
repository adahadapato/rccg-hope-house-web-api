using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces
{
    /// <summary>
    /// Defines persistence operations for prayer requests.
    /// </summary>
    public interface IPrayerRequestRepository
    {
        /// <summary>
        /// Adds a new prayer request to the persistence context.
        /// </summary>
        /// <param name="request">
        /// The prayer request to add.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        Task AddAsync(
            PrayerRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves a prayer request by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the prayer request.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The matching prayer request when found; otherwise, <c>null</c>.
        /// </returns>
        Task<PrayerRequest?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        /// <summary>
        /// Retrieves prayer requests, optionally filtered by status.
        /// </summary>
        /// <param name="status">
        /// The status to filter by.
        /// When <c>null</c>, prayer requests of all statuses are returned.
        /// </param>
        /// <param name="skip">
        /// The number of records to skip.
        /// </param>
        /// <param name="take">
        /// The maximum number of records to return.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// A read-only collection of matching prayer requests.
        /// </returns>
        Task<IReadOnlyList<PrayerRequest>> GetByStatusAsync(
            PrayerRequestStatus? status,
            int skip,
            int take,
            CancellationToken ct = default);

        /// <summary>
        /// Gets the number of prayer requests with the specified status.
        /// </summary>
        /// <param name="status">
        /// The prayer request status to count.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The number of prayer requests with the specified status.
        /// </returns>
        Task<int> GetCountByStatusAsync(
            PrayerRequestStatus status,
            CancellationToken ct = default);

        /// <summary>
        /// Marks an existing prayer request for update.
        /// </summary>
        /// <param name="request">
        /// The prayer request to update.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        Task UpdateAsync(
            PrayerRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Marks a prayer request for deletion.
        /// </summary>
        /// <param name="request">
        /// The prayer request to delete.
        /// </param>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        Task DeleteAsync(
            PrayerRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Persists pending prayer request changes.
        /// </summary>
        /// <param name="ct">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The number of state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync(
            CancellationToken ct = default);
    }
}