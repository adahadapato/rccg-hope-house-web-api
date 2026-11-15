namespace RccgHopeHouse.Core.Interfaces;

/// <summary>
/// Optional unit of work interface for explicit transaction control.
/// Most scenarios can rely on EF Core's implicit unit of work via SaveChangesAsync.
/// Use this only when you need to coordinate multiple repository operations in a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Persists all pending changes (with or without explicit transaction).
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}