using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Auth.Commands;
using RccgHopeHouse.Application.Features.Auth.Dtos;

namespace RccgHopeHouse.Api.Endpoints.Auth;

/// <summary>
/// Minimal API endpoints for authentication: login, refresh, logout.
/// </summary>
public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group)
    {
        var auth = group.MapGroup("/auth")
                        .WithTags("Authentication");

        auth.MapPost("/login", LoginAsync)
            .WithSummary("Authenticate user and return JWT tokens")
            .WithDescription("Validates email/password and returns access/refresh tokens.")
            .WithName("Login")
            .Produces<AuthTokensDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        auth.MapPost("/refresh", RefreshTokenAsync)
            .WithSummary("Exchange refresh token for new access token")
            .WithName("RefreshToken")
            .Produces<AuthTokensDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        auth.MapPost("/logout", LogoutAsync)
            .WithSummary("Invalidate refresh token (logout)")
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return group;
    }

    // ✅ ADD [FromServices] TO ALL HANDLER PARAMETERS THAT COME FROM DI
    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IMediator mediator, // ← FIXED
        CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var tokens = await mediator.Send(command, ct);
        return TypedResults.Ok(tokens);
    }

    private static async Task<IResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IMediator mediator, // ← FIXED
        CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var tokens = await mediator.Send(command, ct);
        return TypedResults.Ok(tokens);
    }

    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        [FromServices] IMediator mediator, // ← FIXED
        CancellationToken ct)
    {
        await mediator.Send(new RevokeTokenCommand(request.RefreshToken), ct);
        return TypedResults.NoContent();
    }
}

// ==================== Request DTOs ====================
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);