using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface IChurchServiceRepository
    {
        Task<IReadOnlyList<ChurchService>> GetAllActiveAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ChurchService>> GetByCategoryAsync(ServiceCategory category, CancellationToken ct = default);
        Task<IReadOnlyList<ChurchService>> GetByDayOfWeekAsync(DayOfWeek day, CancellationToken ct = default);
        Task<ChurchService?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ChurchService service, CancellationToken ct = default);
        Task UpdateAsync(ChurchService service, CancellationToken ct = default);
        Task DeleteAsync(ChurchService service, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IEnumerable<ChurchService>> GetAllAsync(ServiceCategory? category, DayOfWeek? dayOfWeek, bool? isActive, int skip, int take, CancellationToken ct);
    }
}
