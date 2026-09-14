using Microsoft.AspNetCore.Identity;

namespace RccgHopeHouse.Infrastructure.Identity;

/// <summary>
/// ASP.NET Core Identity user entity.
/// Extends IdentityUser to integrate with EF Core while keeping domain entities pure.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// First name for display and email personalization.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name for display and email personalization.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the account is active and permitted to log in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp of the last successful authentication.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}