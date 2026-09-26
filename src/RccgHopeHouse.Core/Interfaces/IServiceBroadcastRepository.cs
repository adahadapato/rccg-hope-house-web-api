using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IServiceBroadcastRepository
    {
        Task<ServiceBroadcast?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default);

        /// <summary>
        /// Gets the current/latest broadcast for a category.
        /// Retained for existing functionality.
        /// </summary>
        Task<ServiceBroadcast?> GetLatestByCategoryAsync(
            ServiceCategory category,
            CancellationToken ct = default);

        /// <summary>
        /// Gets the latest broadcast for each of the supplied
        /// categories. Used by the public Monthly Services section.
        /// </summary>
        Task<IReadOnlyList<ServiceBroadcast>>
            GetLatestForCategoriesAsync(
                IEnumerable<ServiceCategory> categories,
                CancellationToken ct = default);

        /// <summary>
        /// Gets broadcast history for a category.
        /// Retained for existing functionality.
        /// </summary>
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
        Task<IReadOnlyList<ServiceBroadcast>>
            GetAllAsync(
                int skip,
                int take,
                CancellationToken ct = default);

        Task AddAsync(
            ServiceBroadcast broadcast,
            CancellationToken ct = default);

        Task UpdateAsync(
            ServiceBroadcast broadcast,
            CancellationToken ct = default);

        Task DeleteAsync(
            ServiceBroadcast broadcast,
            CancellationToken ct = default);

        Task<int> SaveChangesAsync(
            CancellationToken ct = default);
    }
}