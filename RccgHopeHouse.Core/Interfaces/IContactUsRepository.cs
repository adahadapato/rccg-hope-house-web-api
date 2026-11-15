using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for Contact Us submissions.
/// </summary>
public interface IContactUsRepository
{
    /// <summary>
    /// Retrieves a contact submission by its unique identifier.
    /// </summary>
    /// <param name="id">The contact submission identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// The contact submission when found; otherwise, <c>null</c>.
    /// </returns>
    Task<ContactUs?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves Contact Us submissions using pagination.
    /// </summary>
    /// <param name="skip">
    /// Number of records to skip.
    /// </param>
    /// <param name="take">
    /// Maximum number of records to return.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A read-only collection of contact submissions ordered
    /// from newest to oldest.
    /// </returns>
    Task<IReadOnlyList<ContactUs>> GetAllAsync(
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves contact submissions for a particular contact reason.
    /// </summary>
    /// <param name="reason">
    /// Contact reason to filter by.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A read-only collection of matching contact submissions.
    /// </returns>
    Task<IReadOnlyList<ContactUs>> GetByReasonAsync(
        ContactReason reason,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves unread Contact Us submissions using pagination.
    /// </summary>
    /// <param name="skip">
    /// Number of unread records to skip.
    /// </param>
    /// <param name="take">
    /// Maximum number of unread records to return.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A read-only collection of unread submissions ordered
    /// from newest to oldest.
    /// </returns>
    Task<IReadOnlyList<ContactUs>> GetUnreadAsync(
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Gets the total number of unread Contact Us submissions.
    /// </summary>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The number of unread submissions.
    /// </returns>
    Task<int> GetUnreadCountAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new Contact Us submission.
    /// </summary>
    /// <param name="request">
    /// Contact submission to add.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task AddAsync(
        ContactUs request,
        CancellationToken ct = default);

    /// <summary>
    /// Marks a Contact Us entity for update.
    /// </summary>
    /// <param name="request">
    /// Contact submission to update.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task UpdateAsync(
        ContactUs request,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a Contact Us submission permanently.
    /// </summary>
    /// <param name="contact">
    /// Contact submission to delete.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    Task DeleteAsync(
        ContactUs contact,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending repository changes.
    /// </summary>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Number of affected database records.
    /// </returns>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}