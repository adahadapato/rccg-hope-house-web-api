namespace RccgHopeHouse.Application.Features.Admin.Users.Dtos;

/// <summary>
/// Represents an application user returned to the
/// administration interface.
/// </summary>
public sealed record AdminUserDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    bool IsActive,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles);