namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Current two-factor authentication state.
/// </summary>
public sealed record TwoFactorStatusResult(
    bool IsEnabled,
    bool HasAuthenticator,
    int RecoveryCodesLeft);
