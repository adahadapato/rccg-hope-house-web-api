using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for church services.
/// </summary>
public interface IChurchServiceRepository
{
    /// <summary>
    /// Gets a church service by its unique identifier.
    /// </summary>
    Task<ChurchService?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets church services using optional public and
    /// administrative filters.
    /// </summary>
    Task<IReadOnlyList<ChurchService>> GetAllAsync(ServiceCategory? category, DayOfWeek? dayOfWeek,  bool? isActive,  bool? isLocal, int skip, int take, CancellationToken ct = default);

    /// <summary>
    /// Gets all active church services for which automatic
    /// or manual broadcast association has been enabled.
    /// </summary>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Active church services with broadcasting enabled.
    /// </returns>
    Task<IReadOnlyList<ChurchService>>GetBroadcastEnabledAsync(CancellationToken ct = default);

    /// <summary>
    /// Adds a church service.
    /// </summary>
    Task AddAsync(ChurchService service, CancellationToken ct = default);

    /// <summary>
    /// Marks a church service for update.
    /// </summary>
    Task UpdateAsync(ChurchService service, CancellationToken ct = default);

    /// <summary>
    /// Marks a church service for deletion.
    /// </summary>
    Task DeleteAsync(ChurchService service, CancellationToken ct = default);

    /// <summary>
    /// Persists pending repository changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}