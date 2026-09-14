using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc; // ← Required for ValidationProblemDetails
using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Api.Middleware;

/// <summary>
/// Global exception handler middleware that maps domain and application exceptions
/// to RFC 7807 ProblemDetails responses. Keeps API responses consistent and client-friendly.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        // ✅ Use ValidationProblemDetails for FluentValidation errors
        var problem = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource Not Found",
                Detail = notFound.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Instance = httpContext.Request.Path
            },

            FluentValidation.ValidationException fluent => new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "One or more validation errors occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Instance = httpContext.Request.Path,
                Errors = fluent.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            },

            UnauthorizedAccessException unauthorized => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Authentication required to access this resource.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Instance = httpContext.Request.Path
            },

            DomainException domain => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Domain Rule Violation",
                Detail = domain.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Instance = httpContext.Request.Path
            },

            // Fallback for unexpected errors
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Instance = httpContext.Request.Path
            }
        };

        httpContext.Response.StatusCode = problem.Status!.Value;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}