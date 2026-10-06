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

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="MemberRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// The application database context.
    /// </param>
    public MemberRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Member?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.Members
            .Include(member => member.Photo)
            .FirstOrDefaultAsync(
                member => member.Id == id,
                ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>> GetAllAsync(
        bool includeInactive,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        var query =
            _context.Members
                .Include(member => member.Photo)
                .AsQueryable();

        if (!includeInactive)
        {
            query =
                query.Where(
                    member =>
                        member.IsActive);
        }

        return await query
            .OrderBy(
                member =>
                    member.LastName)
            .ThenBy(
                member =>
                    member.FirstName)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<int> GetActiveCountAsync(
        CancellationToken ct = default) =>
        await _context.Members
            .CountAsync(
                member =>
                    member.IsActive,
                ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>>
        GetMembersWithBirthdayOnAsync(
            int month,
            int day,
            CancellationToken ct = default) =>
        await _context.Members
            .Include(member => member.Photo)
            .Where(
                member =>
                    member.IsActive &&
                    member.ConsentToContact &&
                    member.Birthday != null &&
                    member.Birthday.Month == month &&
                    member.Birthday.Day == day)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Member>>
        GetMembersWithAnniversaryOnAsync(
            int month,
            int day,
            CancellationToken ct = default) =>
        await _context.Members
            .Where(
                member =>
                    member.IsActive &&
                    member.ConsentToContact &&
                    member.WeddingAnniversary != null &&
                    member.WeddingAnniversary.Value.Month ==
                        month &&
                    member.WeddingAnniversary.Value.Day ==
                        day)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task AddAsync(
        Member member,
        CancellationToken ct = default) =>
        await _context.Members.AddAsync(
            member,
            ct);

    /// <inheritdoc />
    public Task UpdateAsync(
        Member member,
        CancellationToken ct = default)
    {
        _context.Members.Update(
            member);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(
        Member member,
        CancellationToken ct = default)
    {
        _context.Members.Remove(
            member);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(
            ct);
}