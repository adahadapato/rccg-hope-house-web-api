using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Provides account-management operations for the
/// currently authenticated user.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Gets the current user's account and profile information.
    /// </summary>
    Task<AccountProfileResult> GetProfileAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Updates the current user's first name and last name.
    /// </summary>
    Task<AccountProfileResult> UpdateProfileAsync(
        string userId,
        string firstName,
        string lastName,
        CancellationToken ct = default);

    /// <summary>
    /// Updates the current user's phone number.
    /// </summary>
    Task<AccountProfileResult> UpdatePhoneNumberAsync(
        string userId,
        string? phoneNumber,
        CancellationToken ct = default);

    /// <summary>
    /// Changes the current user's password after verifying
    /// the existing password.
    /// </summary>
    Task ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default);

    /// <summary>
    /// Stores the relative path of the current user's
    /// profile image.
    /// </summary>
    Task<AccountProfileResult> UpdateProfileImageAsync(
        string userId,
        string? profileImagePath,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the current user's two-factor authentication state.
    /// </summary>
    Task<TwoFactorStatusResult> GetTwoFactorStatusAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Generates the authenticator key and setup information
    /// required to configure an authenticator application.
    /// </summary>
    Task<TwoFactorSetupResult> GetTwoFactorSetupAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Enables two-factor authentication after validating
    /// an authenticator verification code.
    /// </summary>
    Task<IReadOnlyCollection<string>> EnableTwoFactorAsync(
        string userId,
        string verificationCode,
        CancellationToken ct = default);

    /// <summary>
    /// Disables two-factor authentication for the current user.
    /// </summary>
    Task DisableTwoFactorAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Generates a new set of two-factor recovery codes.
    /// </summary>
    Task<IReadOnlyCollection<string>> GenerateRecoveryCodesAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes the currently authenticated user's account
    /// after confirming their password.
    /// </summary>
    Task DeleteAccountAsync(
        string userId,
        string currentPassword,
        CancellationToken ct = default);
}