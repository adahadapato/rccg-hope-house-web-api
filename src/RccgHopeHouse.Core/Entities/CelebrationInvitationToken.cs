
using System.Security.Cryptography;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a secure invitation link issued to a church member
/// for responding to a birthday or wedding anniversary celebration.
///
/// Only the SHA-256 hash of the invitation token is stored in the
/// database. The original token is supplied to the member through
/// a secure invitation link.
///
/// Invitations may expire, be used once, or be revoked.
/// </summary>
public class CelebrationInvitationToken : BaseEntity
{
    /// <summary>
    /// Gets the celebration associated with this invitation.
    /// </summary>
    public Guid CelebrationId { get; private set; }

    /// <summary>
    /// Gets the associated celebration.
    /// </summary>
    public Celebration Celebration { get; private set; } = null!;

    /// <summary>
    /// Gets the SHA-256 hash of the invitation token.
    /// The original token must never be stored in the database.
    /// </summary>
    public string TokenHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the UTC date and time the invitation token was created.
    /// </summary>
    public DateTime IssuedAt { get; private set; }

    /// <summary>
    /// Gets the UTC date and time the invitation expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Gets the UTC date and time the invitation was used.
    /// A null value indicates that no response has been submitted.
    /// </summary>
    public DateTime? UsedAt { get; private set; }

    /// <summary>
    /// Gets the UTC date and time the invitation was revoked.
    /// A null value indicates that it has not been revoked.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the invitation has been used.
    /// </summary>
    public bool IsUsed => UsedAt.HasValue;

    /// <summary>
    /// Gets a value indicating whether the invitation has been revoked.
    /// </summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>
    /// SQL Server concurrency token used to prevent simultaneous
    /// requests from consuming the same invitation.
    /// </summary>
    public byte[] RowVersion { get; private set; } = [];

    /// <summary>
    /// Required by Entity Framework Core.
    /// </summary>
    private CelebrationInvitationToken()
    {
    }

    /// <summary>
    /// Creates a new invitation token record.
    /// </summary>
    /// <param name="celebrationId">
    /// The celebration for which the invitation is issued.
    /// </param>
    /// <param name="tokenHash">
    /// SHA-256 hash of the securely generated invitation token.
    /// </param>
    /// <param name="issuedAtUtc">
    /// The UTC date and time the invitation was issued.
    /// </param>
    /// <param name="expiresAtUtc">
    /// The UTC date and time the invitation expires.
    /// </param>
    /// <returns>
    /// A new invitation token entity.
    /// </returns>
    public static CelebrationInvitationToken Create(
        Guid celebrationId,
        string tokenHash,
        DateTime issuedAtUtc,
        DateTime expiresAtUtc)
    {
        if (celebrationId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid celebration ID is required.",
                nameof(celebrationId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (tokenHash.Length != 64 ||
            !tokenHash.All(Uri.IsHexDigit))
        {
            throw new ArgumentException(
                "The token hash must be a valid SHA-256 hexadecimal value.",
                nameof(tokenHash));
        }

        if (issuedAtUtc.Kind != DateTimeKind.Utc ||
            expiresAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "Invitation timestamps must use UTC.");
        }

        if (expiresAtUtc <= issuedAtUtc)
        {
            throw new ArgumentException(
                "The invitation expiry must be later than its issue time.",
                nameof(expiresAtUtc));
        }

        return new CelebrationInvitationToken
        {
            CelebrationId = celebrationId,
            TokenHash = tokenHash.ToUpperInvariant(),
            IssuedAt = issuedAtUtc,
            ExpiresAt = expiresAtUtc
        };
    }

    /// <summary>
    /// Determines whether the invitation is currently valid.
    /// A valid invitation has not expired, been used or been revoked.
    /// </summary>
    /// <param name="nowUtc">
    /// The current UTC date and time.
    /// </param>
    /// <returns>
    /// True when the invitation is valid; otherwise false.
    /// </returns>
    public bool IsValid(DateTime nowUtc)
    {
        return nowUtc.Kind == DateTimeKind.Utc &&
               nowUtc >= IssuedAt &&
               nowUtc < ExpiresAt &&
               !IsUsed &&
               !IsRevoked &&
               !IsDeleted;
    }

    /// <summary>
    /// Marks the invitation as used after the member's response
    /// has been successfully accepted.
    /// </summary>
    /// <param name="usedAtUtc">
    /// The UTC date and time the invitation was used.
    /// </param>
    public void MarkAsUsed(DateTime usedAtUtc)
    {
        if (!IsValid(usedAtUtc))
        {
            throw new InvalidOperationException(
                "This invitation is expired, revoked, already used or otherwise invalid.");
        }

        UsedAt = usedAtUtc;
        MarkAsUpdated();
    }

    /// <summary>
    /// Revokes an unused invitation, preventing further responses.
    /// </summary>
    /// <param name="revokedAtUtc">
    /// The UTC date and time the invitation was revoked.
    /// </param>
    public void Revoke(DateTime revokedAtUtc)
    {
        if (revokedAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException(
                "The revocation timestamp must use UTC.",
                nameof(revokedAtUtc));
        }

        if (revokedAtUtc < IssuedAt)
        {
            throw new ArgumentException(
                "The revocation timestamp cannot precede issuance.",
                nameof(revokedAtUtc));
        }

        if (IsUsed)
        {
            throw new InvalidOperationException(
                "A used invitation cannot be revoked.");
        }

        if (IsRevoked)
        {
            return;
        }

        RevokedAt = revokedAtUtc;
        MarkAsUpdated();
    }

    /// <summary>
    /// Computes the SHA-256 hash of a plaintext invitation token.
    /// The plaintext token must be generated using a
    /// cryptographically secure random number generator.
    /// </summary>
    /// <param name="token">
    /// The plaintext invitation token.
    /// </param>
    /// <returns>
    /// An uppercase hexadecimal SHA-256 hash.
    /// </returns>
    public static string ComputeTokenHash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        return Convert.ToHexString(
            SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(token)));
    }

    /// <summary>
    /// Generates a cryptographically secure, URL-safe invitation token.
    ///
    /// The token contains 256 bits of randomness and is represented
    /// as a 64-character hexadecimal string.
    /// </summary>
    /// <returns>
    /// A securely generated plaintext invitation token.
    /// </returns>
    public static string GenerateSecureToken()
    {
        return Convert.ToHexString(
            RandomNumberGenerator.GetBytes(32));
    }
}
