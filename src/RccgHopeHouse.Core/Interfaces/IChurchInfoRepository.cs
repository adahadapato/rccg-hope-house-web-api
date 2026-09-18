using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for the church's contact/profile info — a
/// singleton-style record (one row) representing the church's current
/// address, contact methods, and About-section content.
/// </summary>
public interface IChurchInfoRepository
{
    /// <summary>
    /// Gets the church's info record, or null if it hasn't been configured
    /// yet.
    /// </summary>
    Task<ChurchInfo?> GetAsync(CancellationToken ct = default);

    Task AddAsync(ChurchInfo churchInfo, CancellationToken ct = default);
    Task UpdateAsync(ChurchInfo churchInfo, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}