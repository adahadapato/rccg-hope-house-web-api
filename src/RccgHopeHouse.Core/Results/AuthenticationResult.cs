namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Core-level result record for authentication operations.
/// Keeps Core interfaces decoupled from Application-layer DTOs.
/// </summary>
//public record AuthenticationResult(
//    string AccessToken,
//    string RefreshToken,
//    DateTime ExpiresAt,
//    string Role,
//    string UserName);

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
    string UserName);