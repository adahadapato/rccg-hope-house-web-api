using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Infrastructure.Persistence;

namespace RccgHopeHouse.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOfferingRepository"/>.
/// Provides persistence operations for offerings submitted through
/// the Give Online feature.
/// </summary>
public class OfferingRepository : IOfferingRepository
{
    private readonly ApplicationDbContext _context;

    public OfferingRepository(ApplicationDbContext context) =>
        _context = context;

    /// <inheritdoc />
    public async Task<Offering?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default) =>
        await _context.Offerings
            .Include(o => o.GivingType)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Offering>> GetAllAsync(
        int skip,
        int take,
        CancellationToken ct = default) =>
        await _context.Offerings
            .Include(o => o.GivingType)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Offering>> GetByPaymentStatusAsync(
        PaymentStatus status,
        int skip,
        int take,
        CancellationToken ct = default) =>
        await _context.Offerings
            .Include(o => o.GivingType)
            .Where(o => !o.IsDeleted && o.PaymentStatus == status)
            .OrderByDescending(o => o.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Offering>> GetByGivingTypeAsync(
        Guid givingTypeId,
        int skip,
        int take,
        CancellationToken ct = default) =>
        await _context.Offerings
            .Include(o => o.GivingType)
            .Where(o => !o.IsDeleted &&
                        o.GivingTypeId == givingTypeId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Offering?> GetByPaymentReferenceAsync(
        string paymentReference,
        CancellationToken ct = default) =>
        await _context.Offerings
            .Include(o => o.GivingType)
            .FirstOrDefaultAsync(
                o => !o.IsDeleted &&
                     o.PaymentReference == paymentReference,
                ct);

    /// <inheritdoc />
    public async Task AddAsync(
        Offering offering,
        CancellationToken ct = default) =>
        await _context.Offerings.AddAsync(offering, ct);

    /// <inheritdoc />
    public Task UpdateAsync(
        Offering offering,
        CancellationToken ct = default)
    {
        _context.Offerings.Update(offering);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(
        CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}