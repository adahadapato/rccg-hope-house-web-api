
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for member birthday and
/// wedding anniversary celebrations.
/// </summary>
/// <remarks>
/// Provides methods for managing celebration records,
/// retrieving upcoming celebrations, tracking invitations,
/// reviewing member responses, approving publication,
/// and managing published celebrations.
///
/// Implementations are responsible for database access,
/// while business rules remain within the Celebration entity
/// and the application layer.
/// </remarks>
public interface ICelebrationRepository
{
    /// <summary>
    /// Retrieves a celebration by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the celebration.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching celebration, or null if not found.
    /// </returns>
    Task<Celebration?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all celebrations associated with a member.
    /// </summary>
    /// <param name="memberId">
    /// The unique identifier of the member.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of the member's celebrations,
    /// ordered by celebration date in descending order.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetByMemberAsync(
        Guid memberId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a celebration matching a member, occasion,
    /// and celebration date.
    /// </summary>
    /// <param name="memberId">
    /// The unique identifier of the member.
    /// </param>
    /// <param name="occasion">
    /// The celebration occasion, such as Birthday
    /// or WeddingAnniversary.
    /// </param>
    /// <param name="celebrationDate">
    /// The date of the celebration.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching celebration, or null if none exists.
    /// </returns>
    Task<Celebration?> GetByMemberOccasionAndDateAsync(
        Guid memberId,
        string occasion,
        DateOnly celebrationDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves upcoming celebrations within a specified
    /// date range, excluding declined and withdrawn records.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range, inclusive.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range, inclusive.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of upcoming celebrations,
    /// ordered by celebration date.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetUpcomingAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves pending celebrations for which an invitation
    /// has not yet been sent.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range, inclusive.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range, inclusive.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of celebrations awaiting
    /// invitation dispatch.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetUnsentInvitationsAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves celebrations for which an invitation has
    /// been sent but the member has not yet responded.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of celebrations awaiting
    /// member responses.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetAwaitingMemberResponseAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves celebrations approved by members that
    /// are awaiting administrator review and approval.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of celebrations awaiting
    /// administrator approval.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetAwaitingAdminApprovalAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves administrator-approved celebrations that
    /// are ready for publication on or before a specified date.
    /// </summary>
    /// <param name="throughDate">
    /// The latest celebration date eligible for publication.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of celebrations ready
    /// for publication.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetReadyToPublishAsync(
        DateOnly throughDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves published celebrations within a specified
    /// date range.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range, inclusive.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range, inclusive.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of published celebrations,
    /// ordered by celebration date.
    /// </returns>
    Task<IReadOnlyList<Celebration>> GetPublishedAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new celebration to the database context.
    /// </summary>
    /// <param name="celebration">
    /// The celebration entity to add.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <remarks>
    /// The celebration is tracked by the database context.
    /// Changes are persisted when SaveChangesAsync is called.
    /// </remarks>
    Task AddAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing celebration for updating.
    /// </summary>
    /// <param name="celebration">
    /// The celebration entity to update.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <remarks>
    /// Changes are persisted when SaveChangesAsync is called.
    /// </remarks>
    Task UpdateAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes a celebration without physically
    /// removing its record from the database.
    /// </summary>
    /// <param name="celebration">
    /// The celebration entity to soft-delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <remarks>
    /// Uses the existing BaseEntity soft-delete mechanism.
    /// Changes are persisted when SaveChangesAsync is called.
    /// </remarks>
    Task SoftDeleteAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously persists all pending changes tracked
    /// by the underlying database context.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the database save operation.
    /// </param>
    /// <returns>
    /// A task containing the number of state entries
    /// written to the database.
    /// </returns>
    /// <remarks>
    /// This operation commits pending additions, updates,
    /// and soft deletions.
    ///
    /// Other tracked entity changes in the same database
    /// context may also be persisted.
    /// </remarks>
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
