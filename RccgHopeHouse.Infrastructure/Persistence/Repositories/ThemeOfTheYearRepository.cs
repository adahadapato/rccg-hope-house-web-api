using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IThemeOfTheYearRepository"/>.
/// </summary>
public class ThemeOfTheYearRepository : IThemeOfTheYearRepository
{
    private readonly ApplicationDbContext _context;

    public ThemeOfTheYearRepository(ApplicationDbContext context) => _context = context;

    /// <inheritdoc />
    public async Task<ThemeOfTheYear?> GetByYearAsync(int year, CancellationToken ct = default) =>
        await _context.ThemeOfTheYear.FirstOrDefaultAsync(t => t.Year == year, ct);

    /// <inheritdoc />
    public async Task<ThemeOfTheYear?> GetCurrentYearThemeAsync(CancellationToken ct = default) =>
        await GetByYearAsync(DateTime.UtcNow.Year, ct);

    /// <inheritdoc />
    public async Task<ThemeOfTheYear?> GetLatestAsync(CancellationToken ct = default) =>
        await _context.ThemeOfTheYear
            .OrderByDescending(t => t.Year)
            .FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<ThemeOfTheYear>> GetAllAsync(CancellationToken ct = default) =>
        await _context.ThemeOfTheYear
            .OrderByDescending(t => t.Year)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<ThemeOfTheYear?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ThemeOfTheYear.FirstOrDefaultAsync(t => t.Id == id, ct);

    /// <inheritdoc />
    public async Task AddAsync(ThemeOfTheYear theme, CancellationToken ct = default) =>
        await _context.ThemeOfTheYear.AddAsync(theme, ct);

    /// <inheritdoc />
    public Task UpdateAsync(ThemeOfTheYear theme, CancellationToken ct = default)
    {
        _context.ThemeOfTheYear.Update(theme);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(ThemeOfTheYear theme, CancellationToken ct = default)
    {
        _context.ThemeOfTheYear.Remove(theme);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}