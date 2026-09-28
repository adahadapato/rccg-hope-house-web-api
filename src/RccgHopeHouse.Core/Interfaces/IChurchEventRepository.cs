using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

public interface IChurchEventRepository
{
    Task<ChurchEvent?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Fetches church events with optional filtering.
    /// Passing null for a filter means that field
    /// will not be used to restrict the results.
    /// </summary>
    Task<IReadOnlyList<ChurchEvent>> GetAllAsync(
        ServiceCategory? category,
        bool? isActive,
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Returns active events that have not yet finished.
    /// Used by the public Upcoming Events section.
    /// </summary>
    Task<IReadOnlyList<ChurchEvent>> GetUpcomingAsync(
        DateTime currentDateTime,
        int take,
        CancellationToken ct = default);

    Task AddAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default);

    Task UpdateAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default);

    Task DeleteAsync(
        ChurchEvent churchEvent,
        CancellationToken ct = default);

    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}