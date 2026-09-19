using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Defines persistence operations for offerings submitted through
/// the Give Online feature.
/// </summary>
public interface IOfferingRepository
{
    /// <summary>
    /// Gets an offering by its unique identifier.
    /// </summary>
    Task<Offering?> GetByIdAsync(
        Guid id,
        CancellationToken ct = default);

    /// <summary>
    /// Gets a paginated list of offerings.
    /// </summary>
    Task<IReadOnlyList<Offering>> GetAllAsync(
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Gets offerings having the specified payment status.
    /// </summary>
    Task<IReadOnlyList<Offering>> GetByPaymentStatusAsync(
        PaymentStatus status,
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Gets offerings belonging to a particular giving type.
    /// </summary>
    Task<IReadOnlyList<Offering>> GetByGivingTypeAsync(
        Guid givingTypeId,
        int skip,
        int take,
        CancellationToken ct = default);

    /// <summary>
    /// Gets an offering using its external payment reference.
    /// </summary>
    Task<Offering?> GetByPaymentReferenceAsync(
        string paymentReference,
        CancellationToken ct = default);

    /// <summary>
    /// Adds a new offering.
    /// </summary>
    Task AddAsync(
        Offering offering,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing offering.
    /// </summary>
    Task UpdateAsync(
        Offering offering,
        CancellationToken ct = default);

    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}