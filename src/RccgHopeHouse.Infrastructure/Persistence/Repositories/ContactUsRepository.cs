using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IContactRequestRepository"/>.
/// Handles public form submissions and admin inbox management.
/// </summary>
public class ContactUsRepository : IContactUsRepository
{
    private readonly ApplicationDbContext _context;
    public ContactUsRepository(ApplicationDbContext context) => _context = context;



    // Add to existing ContactRequestRepository

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetByStatusAsync(bool isRead, int skip, int take, CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking()
            .Where(c => c.IsRead == isRead)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public Task MarkAsReadAsync(ContactUs request, CancellationToken ct = default)
    {
        request.MarkAsRead();
        _context.ContactRequests.Update(request);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<ContactUs?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetAllAsync(int skip, int take, CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Skip(skip).Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetByReasonAsync(ContactReason reason, CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking()
            .Where(c => c.Reason == reason)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ContactUs>> GetUnreadAsync(CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking()
            .Where(c => !c.IsRead)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<int> GetUnreadCountAsync(CancellationToken ct = default) =>
        await _context.ContactRequests.AsNoTracking().CountAsync(c => !c.IsRead, ct);

    /// <inheritdoc />
    public async Task AddAsync(ContactUs request, CancellationToken ct = default) =>
        await _context.ContactRequests.AddAsync(request, ct);

    /// <inheritdoc />
    public Task UpdateAsync(ContactUs request, CancellationToken ct = default)
    {
        _context.ContactRequests.Update(request);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(ContactUs contact, CancellationToken ct = default)
    {
        _context.ContactRequests.Remove(contact);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}