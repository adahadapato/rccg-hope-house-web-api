namespace RccgHopeHouse.Core.Models.Admin;

/// <summary>
/// Represents an administrator-managed application user
/// without exposing ASP.NET Core Identity types outside Infrastructure.
/// </summary>
public sealed class AdminUser
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public string? ProfileImagePath { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
}