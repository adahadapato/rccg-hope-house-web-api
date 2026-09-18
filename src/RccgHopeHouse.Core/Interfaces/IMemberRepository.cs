using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for church members. Supports admin management,
/// the live "church family size" stat, and birthday/anniversary lookups
/// for automated greeting messages.
/// </summary>
public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Member>> GetAllAsync(bool includeInactive, int skip, int take, CancellationToken ct = default);

    /// <summary>
    /// Live count of active members — the real, always-current number
    /// backing the "church family size" stat on the About section.
    /// </summary>
    Task<int> GetActiveCountAsync(CancellationToken ct = default);

    /// <summary>
    /// Members whose birthday falls on the given month/day, regardless of
    /// birth year (or with no birth year shared at all). Only includes
    /// members with ConsentToContact = true.
    /// </summary>
    Task<IReadOnlyList<Member>> GetMembersWithBirthdayOnAsync(int month, int day, CancellationToken ct = default);

    /// <summary>
    /// Members whose wedding anniversary falls on the given month/day, any
    /// year. Only includes members with ConsentToContact = true.
    /// </summary>
    Task<IReadOnlyList<Member>> GetMembersWithAnniversaryOnAsync(int month, int day, CancellationToken ct = default);

    Task AddAsync(Member member, CancellationToken ct = default);
    Task UpdateAsync(Member member, CancellationToken ct = default);
    Task DeleteAsync(Member member, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}