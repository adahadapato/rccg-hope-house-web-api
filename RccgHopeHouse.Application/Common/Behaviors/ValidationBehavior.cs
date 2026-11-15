using FluentValidation;
using MediatR;

namespace RccgHopeHouse.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that automatically validates requests using FluentValidation.
/// Runs before any handler executes. Returns 400 Bad Request on validation failure.
/// </summary>
/// <typeparam name="TRequest">The request type (command or query).</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the validation behavior.
    /// </summary>
    /// <param name="validators">All registered validators for TRequest (injected by DI).</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validates the request before passing to the handler.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">The next delegate in the pipeline (the handler).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The handler's response if validation passes.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(); // No validators registered for this request type

        var context = new ValidationContext<TRequest>(request);

        // Run all validators and collect failures
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures); // Caught by global API exception handler

        return await next(); // Validation passed; continue to handler
    }
}