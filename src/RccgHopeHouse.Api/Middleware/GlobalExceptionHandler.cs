
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Api.Middleware;

/// <summary>
/// Global exception handler that maps known application,
/// authentication, validation, domain, and database concurrency
/// exceptions to consistent RFC 7807 ProblemDetails responses.
/// </summary>
/// <remarks>
/// Database concurrency conflicts are reported as HTTP 409.
///
/// Invalid or unavailable celebration invitation responses
/// receive a controlled HTTP 400 response without exposing
/// internal invitation or database details.
///
/// Unexpected exceptions continue to receive HTTP 500.
/// </remarks>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initialises the global exception handler.
    /// </summary>
    /// <param name="logger">
    /// Logger used to record application failures.
    /// </param>
    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var isInvitationResponse =
            HttpMethods.IsPost(httpContext.Request.Method) &&
            httpContext.Request.Path.Value?
                .TrimEnd('/')
                .EndsWith(
                    "/celebrations/invitations/respond",
                    StringComparison.OrdinalIgnoreCase) == true;

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
                        "https://httpstatuses.com/404",
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
                        "https://httpstatuses.com/400",
                    Instance =
                        httpContext.Request.Path,
                    Errors =
                        fluent.Errors
                            .GroupBy(
                                error => error.PropertyName)
                            .ToDictionary(
                                group => group.Key,
                                group => group
                                    .Select(error => error.ErrorMessage)
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
                        "https://httpstatuses.com/401",
                    Instance =
                        httpContext.Request.Path
                },

            // Optimistic concurrency protection.
            // This includes simultaneous attempts to
            // consume the same invitation token.
            DbUpdateConcurrencyException =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status409Conflict,
                    Title =
                        "Concurrent Update Conflict",
                    Detail =
                        "This record was changed by another request. Please refresh and try again.",
                    Type =
                        "https://httpstatuses.com/409",
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
                        "https://httpstatuses.com/409",
                    Instance =
                        httpContext.Request.Path
                },

            // The invitation handler throws InvalidOperationException
            // for invalid, expired, revoked or previously used tokens.
            //
            // Restrict this mapping to the invitation response
            // endpoint to avoid changing unrelated API behaviour.
            InvalidOperationException when isInvitationResponse =>
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status400BadRequest,
                    Title =
                        "Invitation Unavailable",
                    Detail =
                        "This invitation is invalid, expired or no longer available.",
                    Type =
                        "https://httpstatuses.com/400",
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
                        "https://httpstatuses.com/500",
                    Instance =
                        httpContext.Request.Path
                }
        };

        if (problem.Status >= 500)
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
