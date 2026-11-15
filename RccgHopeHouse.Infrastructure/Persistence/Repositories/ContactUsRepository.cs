using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides the EF Core persistence implementation for
/// <see cref="IContactUsRepository"/>.
/// </summary>
/// <remarks>
/// Contact Us submissions are used by both the public contact form
/// and the administrative contact inbox.
/// </remarks>
public sealed class ContactUsRepository : IContactUsRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ContactUsRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// Application database context.
    /// </param>
    public ContactUsRepository(
        ApplicationDbContext context)
    {
        _context =
            context
            ?? throw new ArgumentNullException(
                nameof(context));
    }

    /// <inheritdoc />
    public async Task<ContactUs?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(
                contact => contact.Id == id,
                ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetAllAsync(
        int skip,
        int take,
        CancellationToken ct = default)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .OrderByDescending(
                contact => contact.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetByReasonAsync(
        ContactReason reason,
        CancellationToken ct = default)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .Where(
                contact =>
                    contact.Reason == reason)
            .OrderByDescending(
                contact => contact.CreatedAt)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetUnreadAsync(
        int skip,
        int take,
        CancellationToken ct = default)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .Where(
                contact => !contact.IsRead)
            .OrderByDescending(
                contact => contact.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetUnreadCountAsync(
        CancellationToken ct = default)
    {
        return await _context.ContactRequests
            .AsNoTracking()
            .CountAsync(
                contact => !contact.IsRead,
                ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        ContactUs request,
        CancellationToken ct = default)
    {
        await _context.ContactRequests
            .AddAsync(
                request,
                ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        ContactUs request,
        CancellationToken ct = default)
    {
        _context.ContactRequests.Update(
            request);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        ContactUs contact,
        CancellationToken ct = default)
    {
        _context.ContactRequests.Remove(
            contact);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(
            ct);
    }
}