using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IServiceBroadcastRepository
    {
        Task<ServiceBroadcast?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Gets the current (latest ServiceMonth) broadcast for a category —
        /// e.g. "the Holy Ghost Service video for this month." Falls back
        /// to the most recent one if the current month hasn't been set yet.
        /// </summary>
        Task<ServiceBroadcast?> GetLatestByCategoryAsync(ServiceCategory category, CancellationToken ct = default);

        /// <summary>
        /// Gets the latest broadcast for each of the given categories in
        /// one call — powers MonthlyServices.tsx fetching all three at once.
        /// </summary>
        Task<IReadOnlyList<ServiceBroadcast>> GetLatestForCategoriesAsync(
            IEnumerable<ServiceCategory> categories, CancellationToken ct = default);

        Task<IReadOnlyList<ServiceBroadcast>> GetAllByCategoryAsync(
            ServiceCategory category, int skip, int take, CancellationToken ct = default);

        Task AddAsync(ServiceBroadcast broadcast, CancellationToken ct = default);
        Task UpdateAsync(ServiceBroadcast broadcast, CancellationToken ct = default);
        Task DeleteAsync(ServiceBroadcast broadcast, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}