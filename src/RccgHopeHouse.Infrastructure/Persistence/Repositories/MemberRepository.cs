using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IMemberRepository"/>.
/// </summary>
public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Members.FirstOrDefaultAsync(m => m.Id == id, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>> GetAllAsync(bool includeInactive, int skip, int take, CancellationToken ct = default)
    {
        var query = _context.Members.AsQueryable();
        if (!includeInactive) query = query.Where(m => m.IsActive);

        return await query
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetActiveCountAsync(CancellationToken ct = default) =>
        await _context.Members.CountAsync(m => m.IsActive, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>> GetMembersWithBirthdayOnAsync(int month, int day, CancellationToken ct = default) =>
        await _context.Members
            .Where(m => m.IsActive
                     && m.ConsentToContact
                     && m.BirthMonth == month
                     && m.BirthDay == day)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>> GetMembersWithAnniversaryOnAsync(int month, int day, CancellationToken ct = default) =>
        await _context.Members
            .Where(m => m.IsActive
                     && m.ConsentToContact
                     && m.WeddingAnniversary != null
                     && m.WeddingAnniversary.Value.Month == month
                     && m.WeddingAnniversary.Value.Day == day)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddAsync(Member member, CancellationToken ct = default) =>
        await _context.Members.AddAsync(member, ct);

    /// <inheritdoc />
    public Task UpdateAsync(Member member, CancellationToken ct = default)
    {
        _context.Members.Update(member);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Member member, CancellationToken ct = default)
    {
        _context.Members.Remove(member);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}