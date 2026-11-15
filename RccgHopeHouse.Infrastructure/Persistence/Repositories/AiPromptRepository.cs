using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IAiPromptRepository"/>.
/// </summary>
public class AiPromptRepository
    : IAiPromptRepository
{
    private readonly ApplicationDbContext _context;

    public AiPromptRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<AiPromptVersion?> GetActiveAsync(
        string promptKey,
        CancellationToken ct = default)
    {
        return await _context.AiPromptVersions
            .AsNoTracking()
            .Where(v => v.PromptKey == promptKey && v.IsActive)
            .OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AiPromptVersion>> GetAllActiveAsync(
        CancellationToken ct = default)
    {
        return await _context.AiPromptVersions
            .AsNoTracking()
            .Where(v => v.IsActive)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AiPromptVersion>> GetHistoryAsync(
        string promptKey,
        int take,
        CancellationToken ct = default)
    {
        return await _context.AiPromptVersions
            .AsNoTracking()
            .Where(v => v.PromptKey == promptKey)
            .OrderByDescending(v => v.Version)
            .Take(take)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<AiPromptVersion?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _context.AiPromptVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> GetLatestVersionNumberAsync(
        string promptKey,
        CancellationToken ct = default)
    {
        return await _context.AiPromptVersions
            .Where(v => v.PromptKey == promptKey)
            .Select(v => (int?)v.Version)
            .MaxAsync(ct) ?? 0;
    }

    /// <inheritdoc />
    public async Task DeactivateAllAsync(
        string promptKey,
        CancellationToken ct = default)
    {
        var active = await _context.AiPromptVersions
            .Where(v => v.PromptKey == promptKey && v.IsActive)
            .ToListAsync(ct);

        foreach (var version in active)
        {
            version.Deactivate();
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(
        AiPromptVersion version,
        CancellationToken ct = default)
    {
        await _context.AiPromptVersions.AddAsync(version, ct);
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
