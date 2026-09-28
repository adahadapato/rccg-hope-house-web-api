namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Account/profile information belonging to the
/// currently authenticated user.
/// </summary>
public sealed record AccountProfileResult(
    string Id,
    string Email,
    bool EmailConfirmed,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    string? ProfileImagePath,
    bool TwoFactorEnabled,
    DateTime? LastLoginAt);
