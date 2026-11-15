namespace RccgHopeHouse.Core.Exceptions;

/// <summary>
/// Thrown when an optimistic concurrency conflict occurs.
/// Use with EF Core's RowVersion/ConcurrencyToken for safe updates.
/// </summary>
public class ConcurrencyException : DomainException
{
    /// <summary>
    /// Initializes a new instance for a specific entity type.
    /// </summary>
    public ConcurrencyException(string entityName, object id)
        : base($"Concurrency conflict: {entityName} with ID ({id}) was modified by another user.") { }
}