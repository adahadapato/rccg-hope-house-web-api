using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

public interface IFirstTimerRepository
{
    /// <summary>Gets a first-timer for editing (change-tracked).</summary>
    Task<FirstTimer?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Finds an existing record with the same phone number
    /// (change-tracked), so a repeat submission updates it.
    /// </summary>
    Task<FirstTimer?> GetByNormalizedPhoneAsync(
        string normalizedPhoneNumber,
        CancellationToken ct = default);

    /// <summary>Newest visitors first. Read-only.</summary>
    Task<IReadOnlyList<FirstTimer>> GetListAsync(
        FirstTimerFollowUpStatus? status,
        string? search,
        int skip,
        int take,
        CancellationToken ct = default);

    Task<int> GetCountByStatusAsync(
        FirstTimerFollowUpStatus status,
        CancellationToken ct = default);

    Task<int> GetPromotedCountAsync(CancellationToken ct = default);

    Task AddAsync(FirstTimer firstTimer, CancellationToken ct = default);

    Task DeleteAsync(FirstTimer firstTimer, CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
