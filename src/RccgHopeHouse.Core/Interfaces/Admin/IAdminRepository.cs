using RccgHopeHouse.Core.Models.Admin;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Interfaces.Admin;

/// <summary>
/// Defines administrative operations for managing
/// application users and roles.
/// </summary>
public interface IAdminRepository
{
    // ==================== Users ====================

    Task<IReadOnlyList<AdminUser>> GetUsersAsync(
        CancellationToken ct = default);

    Task<AdminUser?> GetUserByIdAsync(
        string userId,
        CancellationToken ct = default);

    Task<AdminUser?> GetUserByEmailAsync(
        EmailAddress email,
        CancellationToken ct = default);

    Task<string> CreateUserAsync(
        EmailAddress email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        string password,
        CancellationToken ct = default);

    Task UpdateUserAsync(
        string userId,
        EmailAddress email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        CancellationToken ct = default);

    Task UpdateUserRolesAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken ct = default);

    Task SetUserActiveStatusAsync(
        string userId,
        bool isActive,
        CancellationToken ct = default);

    Task DeleteUserAsync(
        string userId,
        CancellationToken ct = default);

    Task<bool> IsUserInRoleAsync(
        string userId,
        string role,
        CancellationToken ct = default);

    Task<int> CountActiveUsersInRoleAsync(
        string role,
        CancellationToken ct = default);

    // ==================== Email Verification ====================

    Task<string> GenerateEmailConfirmationTokenAsync(
        string userId,
        CancellationToken ct = default);

    Task ConfirmEmailAsync(
        string userId,
        string token,
        CancellationToken ct = default);

    // ==================== Roles ====================

    Task<IReadOnlyList<AdminRole>> GetRolesAsync(
        CancellationToken ct = default);

    Task<AdminRole?> GetRoleByIdAsync(
        string roleId,
        CancellationToken ct = default);

    Task<AdminRole?> GetRoleByNameAsync(
        string roleName,
        CancellationToken ct = default);

    Task<string> CreateRoleAsync(
        string roleName,
        string? description,
        CancellationToken ct = default);

    Task UpdateRoleAsync(
        string roleId,
        string roleName,
        string? description,
        CancellationToken ct = default);

    Task DeleteRoleAsync(
        string roleId,
        CancellationToken ct = default);

    Task<int> CountUsersInRoleAsync(
        string roleName,
        CancellationToken ct = default);
}