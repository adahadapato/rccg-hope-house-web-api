using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IChurchInfoRepository"/>.
/// Singleton-style: expects at most one ChurchInfo row.
/// </summary>
public class ChurchInfoRepository : IChurchInfoRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ChurchInfoRepository"/> class.
    /// </summary>
    /// <param name="context">
    /// The application database context.
    /// </param>
    public ChurchInfoRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Includes contact methods so callers receive the complete
    /// church profile in a single query.
    /// </remarks>
    public async Task<ChurchInfo?> GetAsync(
        CancellationToken ct = default)
    {
        return await _context.ChurchInfo
            .Include(
                c => c.ContactMethods
                    .OrderBy(m => m.DisplayOrder))
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(
        ChurchInfo churchInfo,
        CancellationToken ct = default)
    {
        await _context.ChurchInfo
            .AddAsync(churchInfo, ct);
    }

    /// <inheritdoc />
    public Task UpdateAsync(
        ChurchInfo churchInfo,
        CancellationToken ct = default)
    {
        _context.ChurchInfo.Update(churchInfo);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task AddContactMethodAsync(
        ChurchContactMethod contactMethod,
        CancellationToken ct = default)
    {
        await _context.ChurchContactMethods
            .AddAsync(contactMethod, ct);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}