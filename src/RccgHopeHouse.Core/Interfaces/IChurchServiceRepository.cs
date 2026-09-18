namespace RccgHopeHouse.Core.Interfaces
{
    public interface IChurchServiceRepository
    {
        Task<ChurchService?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>
        /// Fetches services with optional filtering. Passing null for any
        /// filter parameter means "don't filter on this field" — e.g.
        /// GetAllAsync(isActive: true) returns all active services
        /// regardless of category/day/locality.
        /// </summary>
        Task<IReadOnlyList<ChurchService>> GetAllAsync(
            ServiceCategory? category,
            DayOfWeek? dayOfWeek,
            bool? isActive,
            bool? isLocal,
            int skip,
            int take,
            CancellationToken ct = default);

        Task AddAsync(ChurchService service, CancellationToken ct = default);
        Task UpdateAsync(ChurchService service, CancellationToken ct = default);
        Task DeleteAsync(ChurchService service, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}