using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Api.Middleware;

/// <summary>
/// Global exception handler that maps known application,
/// authentication, validation, and domain exceptions to
/// consistent RFC 7807 ProblemDetails responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            NotFoundException notFound =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status404NotFound,
                    Title =
                        "Resource Not Found",
                    Detail =
                        notFound.Message,
                    Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Instance =
                        httpContext.Request.Path
                },

            FluentValidation.ValidationException fluent =>
                new ValidationProblemDetails
                {
                    Status =
                        StatusCodes.Status400BadRequest,
                    Title =
                        "Validation Failed",
                    Detail =
                        "One or more validation errors occurred.",
                    Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Instance =
                        httpContext.Request.Path,
                    Errors =
                        fluent.Errors
                            .GroupBy(
                                error =>
                                    error.PropertyName)
                            .ToDictionary(
                                group =>
                                    group.Key,
                                group =>
                                    group
                                        .Select(
                                            error =>
                                                error.ErrorMessage)
                                        .ToArray())
                },

            UnauthorizedAccessException unauthorized =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status401Unauthorized,
                    Title =
                        "Authentication Failed",
                    Detail =
                        unauthorized.Message,
                    Type =
                        "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Instance =
                        httpContext.Request.Path
                },

            DomainException domain =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status409Conflict,
                    Title =
                        "Domain Rule Violation",
                    Detail =
                        domain.Message,
                    Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Instance =
                        httpContext.Request.Path
                },

            _ =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status500InternalServerError,
                    Title =
                        "Internal Server Error",
                    Detail =
                        "An unexpected error occurred. Please try again later.",
                    Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Instance =
                        httpContext.Request.Path
                }
        };

        if (
            problem.Status >= 500
        )
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}: {Message}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed with status {StatusCode} while processing {Method} {Path}: {Message}",
                problem.Status,
                httpContext.Request.Method,
                httpContext.Request.Path,
                exception.Message);
        }

        httpContext.Response.StatusCode =
            problem.Status!.Value;

        httpContext.Response.ContentType =
            "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}