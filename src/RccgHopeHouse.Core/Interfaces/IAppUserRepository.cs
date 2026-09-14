using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Repository interface for AppUser domain operations.
/// Note: If using ASP.NET Core Identity, user management is handled by UserManager<T>.
/// This interface is only needed for custom user queries outside Identity.
/// </summary>
public interface IAppUserRepository
{
    /// <summary>
    /// Gets a user by email (case-insensitive).
    /// </summary>
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets all active users.
    /// </summary>
    Task<IReadOnlyList<AppUser>> GetActiveUsersAsync(CancellationToken ct = default);

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    Task UpdateAsync(AppUser user, CancellationToken ct = default);

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}