using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Interfaces.Admin;
using RccgHopeHouse.Core.Models.Admin;
using RccgHopeHouse.Core.ValueObjects;
using RccgHopeHouse.Infrastructure.Identity;
using System.Text;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories.Admin;

/// <summary>
/// ASP.NET Core Identity implementation of <see cref="IAdminRepository"/>.
/// Provides administrative operations for managing application users,
/// roles, and account verification.
/// </summary>
public class AdminRepository : IAdminRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AdminRepository(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ==================== Users ====================

    /// <inheritdoc />
    public async Task<IReadOnlyList<AdminUser>> GetUsersAsync(
        CancellationToken ct = default)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ThenBy(u => u.Email)
            .ToListAsync(ct);

        var result = new List<AdminUser>(users.Count);

        foreach (var user in users)
        {
            ct.ThrowIfCancellationRequested();

            var roles =
                await _userManager.GetRolesAsync(user);

            result.Add(
                MapUser(
                    user,
                    roles));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<AdminUser?> GetUserByIdAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var roles =
            await _userManager.GetRolesAsync(user);

        return MapUser(
            user,
            roles);
    }

    /// <inheritdoc />
    public async Task<AdminUser?> GetUserByEmailAsync(
        EmailAddress email,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await _userManager.FindByEmailAsync(
                email.Value);

        if (user is null)
            return null;

        var roles =
            await _userManager.GetRolesAsync(user);

        return MapUser(
            user,
            roles);
    }

    /// <inheritdoc />
    public async Task<string> CreateUserAsync(
        EmailAddress email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        string password,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            UserName = email.Value,
            Email = email.Value,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phoneNumber?.Value,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            IsActive = true
        };

        var result =
            await _userManager.CreateAsync(
                user,
                password);

        EnsureSucceeded(result);

        return user.Id;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(
        string userId,
        EmailAddress email,
        string firstName,
        string lastName,
        PhoneNumber? phoneNumber,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        var emailChanged =
            !string.Equals(
                user.Email,
                email.Value,
                StringComparison.OrdinalIgnoreCase);

        var oldPhoneNumber =
            user.PhoneNumber;

        var newPhoneNumber =
            phoneNumber?.Value;

        var phoneNumberChanged =
            !string.Equals(
                oldPhoneNumber,
                newPhoneNumber,
                StringComparison.Ordinal);

        user.Email = email.Value;
        user.UserName = email.Value;
        user.FirstName = firstName.Trim();
        user.LastName = lastName.Trim();
        user.PhoneNumber = newPhoneNumber;

        if (emailChanged)
        {
            user.EmailConfirmed = false;
        }

        if (phoneNumberChanged)
        {
            user.PhoneNumberConfirmed = false;
        }

        var result =
            await _userManager.UpdateAsync(user);

        EnsureSucceeded(result);
    }

    /// <inheritdoc />
    public async Task UpdateUserRolesAsync(
        string userId,
        IReadOnlyCollection<string> roles,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        var requestedRoles =
            roles
                .Where(role =>
                    !string.IsNullOrWhiteSpace(role))
                .Select(role =>
                    role.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();

        foreach (var role in requestedRoles)
        {
            ct.ThrowIfCancellationRequested();

            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new InvalidOperationException(
                    $"The role '{role}' does not exist.");
            }
        }

        var currentRoles =
            await _userManager.GetRolesAsync(user);

        var rolesToRemove =
            currentRoles
                .Where(currentRole =>
                    !requestedRoles.Contains(
                        currentRole,
                        StringComparer.OrdinalIgnoreCase))
                .ToArray();

        var rolesToAdd =
            requestedRoles
                .Where(requestedRole =>
                    !currentRoles.Contains(
                        requestedRole,
                        StringComparer.OrdinalIgnoreCase))
                .ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    rolesToRemove);

            EnsureSucceeded(removeResult);
        }

        if (rolesToAdd.Length > 0)
        {
            var addResult =
                await _userManager.AddToRolesAsync(
                    user,
                    rolesToAdd);

            EnsureSucceeded(addResult);
        }
    }

    /// <inheritdoc />
    public async Task SetUserActiveStatusAsync(
        string userId,
        bool isActive,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        user.IsActive = isActive;

        var result =
            await _userManager.UpdateAsync(user);

        EnsureSucceeded(result);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        var result =
            await _userManager.DeleteAsync(user);

        EnsureSucceeded(result);
    }

    /// <inheritdoc />
    public async Task<bool> IsUserInRoleAsync(
        string userId,
        string role,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        return await _userManager.IsInRoleAsync(
            user,
            role);
    }

    /// <inheritdoc />
    public async Task<int> CountActiveUsersInRoleAsync(
        string role,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!await _roleManager.RoleExistsAsync(role))
            return 0;

        var usersInRole =
            await _userManager.GetUsersInRoleAsync(role);

        return usersInRole.Count(
            user => user.IsActive);
    }

    // ==================== Email Verification ====================

    /// <inheritdoc />
    public async Task<string> GenerateEmailConfirmationTokenAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        if (user.EmailConfirmed)
        {
            throw new InvalidOperationException(
                "This user's email address is already confirmed.");
        }

        var token =
            await _userManager
                .GenerateEmailConfirmationTokenAsync(user);

        return Base64UrlEncode(token);
    }

    /// <inheritdoc />
    public async Task ConfirmEmailAsync(
        string userId,
        string token,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user =
            await GetRequiredUserAsync(userId);

        if (user.EmailConfirmed)
            return;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "The email confirmation token is required.");
        }

        string decodedToken;

        try
        {
            decodedToken =
                Base64UrlDecode(token);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                "The email confirmation token is invalid.");
        }

        var result =
            await _userManager.ConfirmEmailAsync(
                user,
                decodedToken);

        EnsureSucceeded(result);
    }

    // ==================== Roles ====================

    /// <inheritdoc />
    public async Task<IReadOnlyList<AdminRole>> GetRolesAsync(
        CancellationToken ct = default)
    {
        return await _roleManager.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new AdminRole
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Description = r.Description
            })
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<AdminRole?> GetRoleByIdAsync(
        string roleId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var role =
            await _roleManager.FindByIdAsync(roleId);

        return role is null
            ? null
            : MapRole(role);
    }

    /// <inheritdoc />
    public async Task<AdminRole?> GetRoleByNameAsync(
        string roleName,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var role =
            await _roleManager.FindByNameAsync(
                roleName);

        return role is null
            ? null
            : MapRole(role);
    }

    /// <inheritdoc />
    public async Task<string> CreateRoleAsync(
        string roleName,
        string? description,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var role =
            new ApplicationRole
            {
                Name = roleName.Trim(),
                Description = NormalizeDescription(
                    description)
            };

        var result =
            await _roleManager.CreateAsync(role);

        EnsureSucceeded(result);

        return role.Id;
    }

    /// <inheritdoc />
    public async Task UpdateRoleAsync(
        string roleId,
        string roleName,
        string? description,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var role =
            await GetRequiredRoleAsync(roleId);

        role.Name =
            roleName.Trim();

        role.Description =
            NormalizeDescription(
                description);

        var result =
            await _roleManager.UpdateAsync(role);

        EnsureSucceeded(result);
    }

    /// <inheritdoc />
    public async Task DeleteRoleAsync(
        string roleId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var role =
            await GetRequiredRoleAsync(roleId);

        var result =
            await _roleManager.DeleteAsync(role);

        EnsureSucceeded(result);
    }

    /// <inheritdoc />
    public async Task<int> CountUsersInRoleAsync(
        string roleName,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (!await _roleManager.RoleExistsAsync(
                roleName))
        {
            return 0;
        }

        var users =
            await _userManager.GetUsersInRoleAsync(
                roleName);

        return users.Count;
    }

    // ==================== Private Helpers ====================

    private async Task<ApplicationUser> GetRequiredUserAsync(
        string userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        return user ??
            throw new KeyNotFoundException(
                $"User with ID '{userId}' was not found.");
    }

    private async Task<ApplicationRole> GetRequiredRoleAsync(
        string roleId)
    {
        var role =
            await _roleManager.FindByIdAsync(roleId);

        return role ??
            throw new KeyNotFoundException(
                $"Role with ID '{roleId}' was not found.");
    }

    private static AdminUser MapUser(
        ApplicationUser user,
        IEnumerable<string> roles)
    {
        return new AdminUser
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed =
                user.PhoneNumberConfirmed,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            EmailConfirmed =
                user.EmailConfirmed,
            TwoFactorEnabled =
                user.TwoFactorEnabled,
            Roles = roles
                .OrderBy(role => role)
                .ToArray()
        };
    }

    private static AdminRole MapRole(
        ApplicationRole role)
    {
        return new AdminRole
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description
        };
    }

    private static string? NormalizeDescription(
        string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    private static void EnsureSucceeded(
        IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors =
            string.Join(
                "; ",
                result.Errors.Select(
                    error =>
                        error.Description));

        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(errors)
                ? "The Identity operation failed."
                : errors);
    }

    /// <summary>
    /// Converts an Identity token into a URL-safe
    /// Base64 encoded value.
    /// </summary>
    private static string Base64UrlEncode(
        string value)
    {
        var bytes =
            Encoding.UTF8.GetBytes(value);

        return Convert
            .ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// Converts a URL-safe Base64 value back into
    /// the original Identity token.
    /// </summary>
    private static string Base64UrlDecode(
        string value)
    {
        var base64 =
            value
                .Replace('-', '+')
                .Replace('_', '/');

        var padding =
            base64.Length % 4;

        if (padding == 2)
        {
            base64 += "==";
        }
        else if (padding == 3)
        {
            base64 += "=";
        }
        else if (padding == 1)
        {
            throw new FormatException(
                "Invalid Base64 URL value.");
        }

        var bytes =
            Convert.FromBase64String(base64);

        return Encoding.UTF8.GetString(bytes);
    }
}