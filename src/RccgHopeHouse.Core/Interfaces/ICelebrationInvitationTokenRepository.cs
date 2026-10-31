namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for secure celebration invitation tokens.
///
/// Invitation tokens provide temporary, single-use access to a member's
/// birthday or wedding anniversary publication invitation.
///
/// Only token hashes are stored in the database. Plaintext invitation
/// tokens must never be persisted.
/// </summary>
public interface ICelebrationInvitationTokenRepository
{
    /// <summary>
    /// Retrieves an invitation using its SHA-256 token hash.
    /// </summary>
    /// <param name="tokenHash">The SHA-256 hash of the invitation token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching invitation, or null when not found.</returns>
    Task<CelebrationInvitationToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an invitation by its unique identifier.
    /// </summary>
    /// <param name="id">The invitation identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The matching invitation, or null when not found.</returns>
    Task<CelebrationInvitationToken?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all non-deleted invitations belonging to a celebration,
    /// ordered from the most recently issued to the oldest.
    /// </summary>
    /// <param name="celebrationId">The celebration identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invitation records associated with the celebration.</returns>
    Task<IReadOnlyList<CelebrationInvitationToken>> GetByCelebrationAsync(
        Guid celebrationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves unused, unrevoked and unexpired invitations for a celebration.
    /// These records can be revoked when a replacement invitation is issued.
    /// </summary>
    /// <param name="celebrationId">The celebration identifier.</param>
    /// <param name="nowUtc">The current UTC date and time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The currently valid invitations for the celebration.</returns>
    Task<IReadOnlyList<CelebrationInvitationToken>> GetActiveByCelebrationAsync(
        Guid celebrationId,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new invitation token to the persistence context.
    /// Changes are persisted when SaveChangesAsync is called.
    /// </summary>
    /// <param name="invitation">The invitation to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(
        CelebrationInvitationToken invitation,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing invitation as modified in the persistence context.
    /// Changes are persisted when SaveChangesAsync is called.
    /// </summary>
    /// <param name="invitation">The invitation to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(
        CelebrationInvitationToken invitation,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists pending invitation changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of affected tracked entries.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
