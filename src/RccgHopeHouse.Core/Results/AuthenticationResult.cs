namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Result record for authentication operations.
/// Keeps Core pure by avoiding Application DTO references.
/// </summary>
public record AuthResult(
    bool IsSuccess,
    string? AccessToken,
    string? RefreshToken,
    DateTime? ExpiresAt,
    string? Role,
    string? ErrorMessage,
    string? UserName,
    string? Name,
    string? Email);