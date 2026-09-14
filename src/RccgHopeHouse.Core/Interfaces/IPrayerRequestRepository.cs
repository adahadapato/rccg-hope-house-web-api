using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IPrayerRequestRepository
    {
        Task AddAsync(PrayerRequest request, CancellationToken ct = default);
        Task<PrayerRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<PrayerRequest>> GetByStatusAsync(PrayerRequestStatus? status, int skip, int take, CancellationToken ct = default);
        Task<int> GetCountByStatusAsync(PrayerRequestStatus status, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task UpdateAsync(PrayerRequest request, CancellationToken ct = default);
    }
}
