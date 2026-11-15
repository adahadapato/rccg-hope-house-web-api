using Microsoft.AspNetCore.Identity;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.Admin;
using RccgHopeHouse.Core.Results;
using RccgHopeHouse.Infrastructure.Identity;
using System.Globalization;
using System.Text;

namespace RccgHopeHouse.Infrastructure.Services;

/// <summary>
/// Implements personal account-management operations
/// for the currently authenticated user.
/// </summary>
public class AccountService : IAccountService
{
    private const int RecoveryCodeCount = 10;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAdminRepository _adminRepository;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        IAdminRepository adminRepository)
    {
        _userManager = userManager;
        _adminRepository = adminRepository;
    }

    /// <inheritdoc />
    public async Task<AccountProfileResult> GetProfileAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        return MapProfile(user);
    }

    /// <inheritdoc />
    public async Task<AccountProfileResult> UpdateProfileAsync(
        string userId,
        string firstName,
        string lastName,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        firstName = firstName?.Trim() ?? string.Empty;
        lastName = lastName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new InvalidOperationException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new InvalidOperationException(
                "Last name is required.");
        }

        user.FirstName = firstName;
        user.LastName = lastName;

        var result =
            await _userManager.UpdateAsync(user);

        EnsureIdentitySuccess(
            result,
            "Unable to update the profile.");

        return MapProfile(user);
    }

    /// <inheritdoc />
    public async Task<AccountProfileResult> UpdatePhoneNumberAsync(
        string userId,
        string? phoneNumber,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        var normalizedPhoneNumber =
            string.IsNullOrWhiteSpace(phoneNumber)
                ? null
                : phoneNumber.Trim();

        var result =
            await _userManager.SetPhoneNumberAsync(
                user,
                normalizedPhoneNumber);

        EnsureIdentitySuccess(
            result,
            "Unable to update the phone number.");

        return MapProfile(user);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            throw new InvalidOperationException(
                "Current password is required.");
        }

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            throw new InvalidOperationException(
                "New password is required.");
        }

        var user = await GetUserAsync(userId, ct);

        var result =
            await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

        EnsureIdentitySuccess(
            result,
            "Unable to change the password.");
    }

    /// <inheritdoc />
    public async Task<AccountProfileResult> UpdateProfileImageAsync(
        string userId,
        string? profileImagePath,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        user.ProfileImagePath =
            string.IsNullOrWhiteSpace(profileImagePath)
                ? null
                : profileImagePath.Trim();

        var result =
            await _userManager.UpdateAsync(user);

        EnsureIdentitySuccess(
            result,
            "Unable to update the profile image.");

        return MapProfile(user);
    }

    /// <inheritdoc />
    public async Task<TwoFactorStatusResult> GetTwoFactorStatusAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        var authenticatorKey =
            await _userManager.GetAuthenticatorKeyAsync(user);

        var recoveryCodesLeft =
            await _userManager.CountRecoveryCodesAsync(user);

        return new TwoFactorStatusResult(
            user.TwoFactorEnabled,
            !string.IsNullOrWhiteSpace(authenticatorKey),
            recoveryCodesLeft);
    }

    /// <inheritdoc />
    public async Task<TwoFactorSetupResult> GetTwoFactorSetupAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        var authenticatorKey =
            await _userManager.GetAuthenticatorKeyAsync(user);

        if (string.IsNullOrWhiteSpace(authenticatorKey))
        {
            var resetResult =
                await _userManager.ResetAuthenticatorKeyAsync(
                    user);

            EnsureIdentitySuccess(
                resetResult,
                "Unable to create an authenticator key.");

            authenticatorKey =
                await _userManager.GetAuthenticatorKeyAsync(
                    user);
        }

        if (string.IsNullOrWhiteSpace(authenticatorKey))
        {
            throw new InvalidOperationException(
                "Unable to create an authenticator key.");
        }

        var email =
            user.Email ??
            user.UserName ??
            user.Id;

        var authenticatorUri =
            GenerateAuthenticatorUri(
                email,
                authenticatorKey);

        return new TwoFactorSetupResult(
            FormatKey(authenticatorKey),
            authenticatorUri);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>>
        EnableTwoFactorAsync(
            string userId,
            string verificationCode,
            CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        if (string.IsNullOrWhiteSpace(verificationCode))
        {
            throw new InvalidOperationException(
                "Verification code is required.");
        }

        var code =
            verificationCode
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);

        var isValid =
            await _userManager
                .VerifyTwoFactorTokenAsync(
                    user,
                    _userManager.Options.Tokens
                        .AuthenticatorTokenProvider,
                    code);

        if (!isValid)
        {
            throw new InvalidOperationException(
                "The authenticator verification code is invalid.");
        }

        var enableResult =
            await _userManager.SetTwoFactorEnabledAsync(
                user,
                true);

        EnsureIdentitySuccess(
            enableResult,
            "Unable to enable two-factor authentication.");

        var recoveryCodes =
            await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(
                user,
                RecoveryCodeCount);

        if (recoveryCodes is null)
        {
            throw new InvalidOperationException(
                "Two-factor authentication was enabled, but recovery codes could not be generated.");
        }

        return recoveryCodes.ToArray();
    }

    /// <inheritdoc />
    public async Task DisableTwoFactorAsync(
        string userId,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        var result =
            await _userManager.SetTwoFactorEnabledAsync(
                user,
                false);

        EnsureIdentitySuccess(
            result,
            "Unable to disable two-factor authentication.");
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<string>>
        GenerateRecoveryCodesAsync(
            string userId,
            CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var user = await GetUserAsync(userId, ct);

        if (!user.TwoFactorEnabled)
        {
            throw new InvalidOperationException(
                "Two-factor authentication must be enabled before recovery codes can be generated.");
        }

        var recoveryCodes =
            await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(
                user,
                RecoveryCodeCount);

        if (recoveryCodes is null)
        {
            throw new InvalidOperationException(
                "Unable to generate recovery codes.");
        }

        return recoveryCodes.ToArray();
    }

    /// <inheritdoc />
    public async Task DeleteAccountAsync(
        string userId,
        string currentPassword,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            throw new InvalidOperationException(
                "Current password is required.");
        }

        var user = await GetUserAsync(userId, ct);

        var passwordIsValid =
            await _userManager.CheckPasswordAsync(
                user,
                currentPassword);

        if (!passwordIsValid)
        {
            throw new UnauthorizedAccessException(
                "The current password is incorrect.");
        }

        var isAdmin =
            await _adminRepository.IsUserInRoleAsync(
                userId,
                Roles.Admin,
                ct);

        if (isAdmin && user.IsActive)
        {
            var activeAdminCount =
                await _adminRepository.CountActiveUsersInRoleAsync(
                    Roles.Admin,
                    ct);

            if (activeAdminCount <= 1)
            {
                throw new DomainException(
                    "The last active administrator cannot delete their account.");
            }
        }

        var result =
            await _userManager.DeleteAsync(user);

        EnsureIdentitySuccess(
            result,
            "Unable to delete the account.");
    }

    /// <summary>
    /// Gets the account associated with the supplied
    /// authenticated user identifier.
    /// </summary>
    private async Task<ApplicationUser> GetUserAsync(
        string userId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user's identifier is missing.");
        }

        var user =
            await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new NotFoundException(
                nameof(ApplicationUser),
                userId);
        }

        return user;
    }

    /// <summary>
    /// Maps the Identity user to the account profile
    /// result exposed to the application layer.
    /// </summary>
    private static AccountProfileResult MapProfile(
        ApplicationUser user)
    {
        return new AccountProfileResult(
            user.Id,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.ProfileImagePath,
            user.TwoFactorEnabled,
            user.LastLoginAt);
    }

    /// <summary>
    /// Converts Identity errors into a single exception
    /// that can be handled by the API exception middleware.
    /// </summary>
    private static void EnsureIdentitySuccess(
        IdentityResult result,
        string fallbackMessage)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors =
            result.Errors
                .Select(error => error.Description)
                .Where(description =>
                    !string.IsNullOrWhiteSpace(description))
                .ToArray();

        var message =
            errors.Length > 0
                ? string.Join(" ", errors)
                : fallbackMessage;

        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Formats the authenticator key into groups to make
    /// manual entry easier for the user.
    /// </summary>
    private static string FormatKey(
        string unformattedKey)
    {
        var result = new StringBuilder();

        var currentPosition = 0;

        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(
                unformattedKey.AsSpan(
                    currentPosition,
                    4));

            result.Append(' ');

            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(
                unformattedKey.AsSpan(
                    currentPosition));
        }

        return result
            .ToString()
            .ToLowerInvariant();
    }

    /// <summary>
    /// Generates an otpauth URI understood by common
    /// authenticator applications.
    /// </summary>
    private static string GenerateAuthenticatorUri(
        string email,
        string unformattedKey)
    {
        const string issuer =
            "RCCG Hope House";

        return string.Format(
            CultureInfo.InvariantCulture,
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            Uri.EscapeDataString(issuer),
            Uri.EscapeDataString(email),
            unformattedKey);
    }
}