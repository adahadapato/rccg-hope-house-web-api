using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Application.Features.Auth.Commands;
using RccgHopeHouse.Application.Features.Auth.Dtos;

namespace RccgHopeHouse.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(
        this RouteGroupBuilder group)
    {
        var auth = group
            .MapGroup("/auth")
            .WithTags("Authentication");

        auth.MapPost("/login", LoginAsync)
            .WithSummary(
                "Authenticate user and return JWT tokens")
            .WithDescription(
                "Validates email/password. If two-factor authentication is enabled, returns a short-lived two-factor challenge instead of JWT tokens.")
            .WithName("Login")
            .Produces<AuthTokensDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        auth.MapPost(
                "/two-factor",
                CompleteTwoFactorLoginAsync)
            .WithSummary(
                "Complete two-factor authentication")
            .WithDescription(
                "Validates an authenticator or recovery code for a pending login challenge and returns JWT access/refresh tokens.")
            .WithName("CompleteTwoFactorLogin")
            .Produces<AuthTokensDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        auth.MapGet("/validate", ValidateAdminAsync)
            .WithSummary(
                "Validate the current administrator access token")
            .WithName("ValidateAdminToken")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .ProducesProblem(
                StatusCodes.Status403Forbidden)
            .RequireAuthorization(
                policy =>
                    policy.RequireRole("Admin"));

        auth.MapPost("/refresh", RefreshTokenAsync)
            .WithSummary(
                "Exchange refresh token for new access token")
            .WithName("RefreshToken")
            .Produces<AuthTokensDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        auth.MapPost("/logout", LogoutAsync)
            .WithSummary(
                "Invalidate refresh token (logout)")
            .WithName("Logout")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

       

        /*
         * GET email confirmation endpoint.
         *
         * Used by links contained directly in verification
         * emails. The user ID and token are supplied through
         * the query string.
         */
        auth.MapGet(
                "/confirm-email",
                ConfirmEmailAsync)
            .WithSummary(
                "Confirm a user's email address from an email link")
            .WithDescription(
                "Validates an email confirmation token supplied in the query string and marks the user's email address as confirmed.")
            .WithName("ConfirmEmailFromLink")
            .Produces(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .RequireRateLimiting("Strict")
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        var tokens =
            await mediator.Send(
                new LoginCommand(
                    request.Email,
                    request.Password),
                ct);

        return TypedResults.Ok(tokens);
    }

    private static async Task<IResult> CompleteTwoFactorLoginAsync(
        [FromBody] CompleteTwoFactorLoginRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        var tokens =
            await mediator.Send(
                new CompleteTwoFactorLoginCommand(
                    request.ChallengeToken,
                    request.VerificationCode,
                    request.UseRecoveryCode),
                ct);

        return TypedResults.Ok(tokens);
    }

    private static IResult ValidateAdminAsync() =>
        TypedResults.NoContent();

    private static async Task<IResult> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        var tokens =
            await mediator.Send(
                new RefreshTokenCommand(
                    request.RefreshToken),
                ct);

        return TypedResults.Ok(tokens);
    }

    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(
            new RevokeTokenCommand(
                request.RefreshToken),
            ct);

        return TypedResults.NoContent();
    }

   private static async Task<IResult> ConfirmEmailAsync(
        [FromQuery] string userId,
        [FromQuery] string token,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new ConfirmAdminUserEmailCommand(userId, token),ct);

        return TypedResults.Ok(
            new
            {
                message = "Your email address has been verified successfully."
            });
    }
}

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record CompleteTwoFactorLoginRequest(
    string ChallengeToken,
    string VerificationCode,
    bool UseRecoveryCode);

public sealed record RefreshTokenRequest(
    string RefreshToken);

public sealed record LogoutRequest(
    string RefreshToken);

