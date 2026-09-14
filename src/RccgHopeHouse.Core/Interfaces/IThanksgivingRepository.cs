using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IThanksgivingRepository
    {
        Task<ThanksgivingService?> GetByMonthAsync(DateTime month, CancellationToken ct = default);
        Task<IReadOnlyList<ThanksgivingService>> GetAsync(DateTime fromMonth, DateTime toMonth, CancellationToken ct = default);
        Task AddAsync(ThanksgivingService service, CancellationToken ct = default);
        Task UpdateAsync(ThanksgivingService service, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task <ThanksgivingService?> GetByVideoUrlAsync(string videoUrl, CancellationToken ct);
    }
}
