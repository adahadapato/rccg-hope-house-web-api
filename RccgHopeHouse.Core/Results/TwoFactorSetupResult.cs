namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Information required to configure an authenticator app.
/// </summary>
public sealed record TwoFactorSetupResult(
    string SharedKey,
    string AuthenticatorUri);
