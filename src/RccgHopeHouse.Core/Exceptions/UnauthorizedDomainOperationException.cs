namespace RccgHopeHouse.Core.Exceptions;

/// <summary>
/// Thrown when a domain operation violates authorization rules.
/// Example: Non-admin trying to publish a PastorPost.
/// </summary>
public class UnauthorizedDomainOperationException : DomainException
{
    /// <summary>
    /// Initializes a new instance with operation details.
    /// </summary>
    public UnauthorizedDomainOperationException(string operation, string requiredRole)
        : base($"Operation '{operation}' requires role '{requiredRole}'.") { }
}