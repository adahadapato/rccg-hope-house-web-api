using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces
{
    public interface ISermonRepository
    {
        Task<Sermon?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Sermon>> GetAllPublishedAsync(int skip, int take, CancellationToken ct = default);
        Task<int> CountPublishedAsync(CancellationToken ct = default);
        Task AddAsync(Sermon sermon, CancellationToken ct = default);
        Task UpdateAsync(Sermon sermon, CancellationToken ct = default);
        Task DeleteAsync(Sermon sermon, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Sermon>> GetPagedAsync(int skip,
            int take,
            string? search,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken ct = default);
    }
}
