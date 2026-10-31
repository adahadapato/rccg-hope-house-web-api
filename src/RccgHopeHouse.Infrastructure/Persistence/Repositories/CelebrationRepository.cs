
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides the Entity Framework Core implementation of
/// celebration persistence and workflow queries.
/// </summary>
/// <remarks>
/// Manages birthday and wedding anniversary celebration
/// records using the application's existing database context.
///
/// Supports invitation tracking, member consent,
/// administrator approval, publication, and soft deletion.
///
/// Relies on the existing BaseEntity soft-delete query filter.
/// </remarks>
public sealed class CelebrationRepository : ICelebrationRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="CelebrationRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// The application database context.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the database context is null.
    /// </exception>
    public CelebrationRepository(
        ApplicationDbContext context)
    {
        _context = context
            ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a celebration by its unique identifier,
    /// including its associated member and gallery category.
    /// </summary>
    /// <param name="id">
    /// The celebration identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching celebration, or null if not found.
    /// </returns>
    public Task<Celebration?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        _context.Celebrations
            .Include(x => x.Member)
            .Include(x => x.GalleryCategory)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

    /// <summary>
    /// Retrieves all celebrations belonging to a member.
    /// </summary>
    /// <param name="memberId">
    /// The member identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection ordered by celebration date
    /// in descending order, followed by occasion.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetByMemberAsync(
        Guid memberId,
        CancellationToken cancellationToken = default) =>
        await _context.Celebrations
            .AsNoTracking()
            .Where(x => x.MemberId == memberId)
            .OrderByDescending(x => x.CelebrationDate)
            .ThenBy(x => x.Occasion)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Finds an existing celebration using the member,
    /// occasion, and celebration date.
    /// </summary>
    /// <param name="memberId">
    /// The member identifier.
    /// </param>
    /// <param name="occasion">
    /// The occasion, such as Birthday or WeddingAnniversary.
    /// </param>
    /// <param name="celebrationDate">
    /// The celebration date.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The matching celebration, or null if none exists.
    /// </returns>
    public Task<Celebration?> GetByMemberOccasionAndDateAsync(
        Guid memberId,
        string occasion,
        DateOnly celebrationDate,
        CancellationToken cancellationToken = default) =>
        _context.Celebrations
            .FirstOrDefaultAsync(
                x => x.MemberId == memberId
                     && x.Occasion == occasion
                     && x.CelebrationDate == celebrationDate,
                cancellationToken);

    /// <summary>
    /// Retrieves upcoming celebrations within a date range,
    /// excluding declined and withdrawn celebrations.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of upcoming celebrations.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetUpcomingAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default)
    {
        ValidateRange(fromDate, throughDate);

        return await _context.Celebrations
            .AsNoTracking()
            .Where(x =>
                x.CelebrationDate >= fromDate &&
                x.CelebrationDate <= throughDate &&
                x.Status != "Declined" &&
                x.Status != "Withdrawn")
            .OrderBy(x => x.CelebrationDate)
            .ThenBy(x => x.MemberId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves pending celebrations for which invitations
    /// have not yet been sent.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of unsent invitations.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetUnsentInvitationsAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default)
    {
        ValidateRange(fromDate, throughDate);

        return await _context.Celebrations
            .Where(x =>
                x.CelebrationDate >= fromDate &&
                x.CelebrationDate <= throughDate &&
                x.Status == "Pending" &&
                x.InvitationSentAt == null)
            .OrderBy(x => x.CelebrationDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves celebrations where invitations have been
    /// sent but members have not yet responded.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of pending member responses.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetAwaitingMemberResponseAsync(
        CancellationToken cancellationToken = default) =>
        await _context.Celebrations
            .AsNoTracking()
            .Where(x =>
                x.Status == "Pending" &&
                x.InvitationSentAt != null)
            .OrderBy(x => x.InvitationSentAt)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Retrieves celebrations approved by members that
    /// require administrator approval.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection awaiting administrator approval.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetAwaitingAdminApprovalAsync(
        CancellationToken cancellationToken = default) =>
        await _context.Celebrations
            .AsNoTracking()
            .Where(x => x.Status == "ApprovedByMember")
            .OrderBy(x => x.CelebrationDate)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Retrieves administrator-approved celebrations
    /// eligible for publication on or before a given date.
    /// </summary>
    /// <param name="throughDate">
    /// The latest eligible celebration date.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of celebrations ready
    /// for publication.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetReadyToPublishAsync(
        DateOnly throughDate,
        CancellationToken cancellationToken = default) =>
        await _context.Celebrations
            .Where(x =>
                x.Status == "ReadyToPublish" &&
                x.CelebrationDate <= throughDate)
            .OrderBy(x => x.CelebrationDate)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Retrieves published celebrations within a
    /// specified date range.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A read-only collection of published celebrations.
    /// </returns>
    public async Task<IReadOnlyList<Celebration>> GetPublishedAsync(
        DateOnly fromDate,
        DateOnly throughDate,
        CancellationToken cancellationToken = default)
    {
        ValidateRange(fromDate, throughDate);

        return await _context.Celebrations
            .AsNoTracking()
            .Where(x =>
                x.Status == "Published" &&
                x.CelebrationDate >= fromDate &&
                x.CelebrationDate <= throughDate)
            .OrderBy(x => x.CelebrationDate)
            .ToListAsync(cancellationToken);
    }

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
    /// The entity is tracked as Added.
    /// Call SaveChangesAsync to persist the new record.
    /// </remarks>
    public async Task AddAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(celebration);

        await _context.Celebrations.AddAsync(
            celebration,
            cancellationToken);
    }

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
    /// Call SaveChangesAsync to persist the update.
    /// </remarks>
    public Task UpdateAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(celebration);

        _context.Celebrations.Update(celebration);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Soft-deletes an existing celebration using the
    /// BaseEntity soft-delete mechanism.
    /// </summary>
    /// <param name="celebration">
    /// The celebration entity to soft-delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the operation.
    /// </param>
    /// <remarks>
    /// The record remains in the database but is excluded
    /// by the application's soft-delete query filter.
    ///
    /// Call SaveChangesAsync to persist the change.
    /// </remarks>
    public Task SoftDeleteAsync(
        Celebration celebration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(celebration);

        celebration.SoftDelete();
        celebration.MarkAsUpdated();

        _context.Celebrations.Update(celebration);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously saves all pending changes tracked by
    /// the application database context.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel
    /// the database save operation.
    /// </param>
    /// <returns>
    /// A task containing the number of state entries
    /// written to the database.
    /// </returns>
    /// <remarks>
    /// Persists pending celebration additions, updates,
    /// and soft deletions.
    ///
    /// Uses the existing ApplicationDbContext and follows
    /// the same persistence pattern as the MemberRepository.
    ///
    /// Other tracked entity changes in the same context
    /// may also be saved.
    /// </remarks>
    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(
            cancellationToken);
    }

    /// <summary>
    /// Validates that a date range has a valid start
    /// and end date.
    /// </summary>
    /// <param name="fromDate">
    /// The beginning of the date range.
    /// </param>
    /// <param name="throughDate">
    /// The end of the date range.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when the end date precedes the start date.
    /// </exception>
    private static void ValidateRange(
        DateOnly fromDate,
        DateOnly throughDate)
    {
        if (throughDate < fromDate)
        {
            throw new ArgumentException(
                "End date must not precede start date.",
                nameof(throughDate));
        }
    }
}
