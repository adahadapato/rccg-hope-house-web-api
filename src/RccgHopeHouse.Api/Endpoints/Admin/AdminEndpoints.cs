using MediatR;
using Microsoft.Extensions.Configuration;
using RccgHopeHouse.Application.Features.Admin.Roles.Commands;
using RccgHopeHouse.Application.Features.Admin.Roles.Queries;
using RccgHopeHouse.Application.Features.Admin.Users.Commands;
using RccgHopeHouse.Application.Features.Admin.Users.Queries;
using RccgHopeHouse.Core.Constants;
using System.Security.Claims;

namespace RccgHopeHouse.Api.Endpoints.Admin;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(
        this RouteGroupBuilder group)
    {
        var admin = group
            .MapGroup("/admin")
            .WithTags("Admin")
            .RequireAuthorization(Policies.RequireAdmin);

        // ==================== Users ====================

        admin.MapGet(
                "/users",
                GetUsersAsync)
            .WithName("GetAdminUsers");

        admin.MapPost(
                "/users",
                CreateUserAsync)
            .WithName("CreateAdminUser");

        admin.MapPut(
                "/users/{userId}",
                UpdateUserAsync)
            .WithName("UpdateAdminUser");

        admin.MapPut(
                "/users/{userId}/roles",
                UpdateUserRolesAsync)
            .WithName("UpdateAdminUserRoles");

        admin.MapPut(
                "/users/{userId}/status",
                SetUserStatusAsync)
            .WithName("SetAdminUserStatus");

        admin.MapDelete(
                "/users/{userId}",
                DeleteUserAsync)
            .WithName("DeleteAdminUser")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .ProducesProblem(
                StatusCodes.Status403Forbidden)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status409Conflict);

        admin.MapPost(
                "/users/{userId}/send-verification-email",
                SendVerificationEmailAsync)
            .WithSummary(
                "Send or resend an email verification message")
            .WithDescription(
                "Generates a new email confirmation token and sends a verification email to the user.")
            .WithName("SendAdminUserVerificationEmail")
            .Produces<VerificationEmailResponse>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .ProducesProblem(
                StatusCodes.Status403Forbidden)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        // ==================== Roles ====================

        admin.MapGet(
                "/roles",
                GetRolesAsync)
            .WithName("GetAdminRoles");

        admin.MapPost(
                "/roles",
                CreateRoleAsync)
            .WithName("CreateAdminRole");

        admin.MapPut(
                "/roles/{roleId}",
                UpdateRoleAsync)
            .WithName("UpdateAdminRole");

        admin.MapDelete(
                "/roles/{roleId}",
                DeleteRoleAsync)
            .WithName("DeleteAdminRole");

        return group;
    }

    // ==================== User Handlers ====================

    private static async Task<IResult> GetUsersAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var users =
            await mediator.Send(
                new GetAdminUsersQuery(),
                cancellationToken);

        return Results.Ok(users);
    }

    private static async Task<IResult> CreateUserAsync(
        CreateAdminUserCommand command,
        IMediator mediator,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var userId =
            await mediator.Send(
                command,
                cancellationToken);

        var verificationBaseUrl =
            GetVerificationBaseUrl(configuration);

        var emailResult =
            await mediator.Send(
                new SendAdminUserVerificationEmailCommand(
                    userId,
                    verificationBaseUrl),
                cancellationToken);

        var response =
            new CreateAdminUserResponse(
                Id: userId,
                VerificationEmailSent:
                    emailResult.IsSuccess,
                VerificationEmailError:
                    emailResult.IsSuccess
                        ? null
                        : emailResult.ErrorMessage);

        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateUserAsync(
        string userId,
        UpdateAdminUserRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new UpdateAdminUserCommand(
                userId,
                request.Email,
                request.FirstName,
                request.LastName,
                request.PhoneNumber);

        await mediator.Send(
            command,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> UpdateUserRolesAsync(
        string userId,
        UpdateAdminUserRolesRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var currentUserId =
            GetCurrentUserId(principal);

        var command =
            new UpdateAdminUserRolesCommand(
                userId,
                currentUserId,
                request.Roles);

        await mediator.Send(
            command,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> SetUserStatusAsync(
        string userId,
        SetAdminUserStatusRequest request,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var currentUserId =
            GetCurrentUserId(principal);

        var command =
            new SetAdminUserStatusCommand(
                userId,
                currentUserId,
                request.IsActive);

        await mediator.Send(
            command,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUserAsync(
        string userId,
        ClaimsPrincipal principal,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var currentUserId =
            GetCurrentUserId(principal);

        var command =
            new DeleteAdminUserCommand(
                userId,
                currentUserId);

        await mediator.Send(
            command,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> SendVerificationEmailAsync(
        string userId,
        IMediator mediator,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var verificationBaseUrl =
            GetVerificationBaseUrl(configuration);

        var emailResult =
            await mediator.Send(
                new SendAdminUserVerificationEmailCommand(
                    userId,
                    verificationBaseUrl),
                cancellationToken);

        var response =
            new VerificationEmailResponse(
                Sent: emailResult.IsSuccess,
                ErrorMessage:
                    emailResult.IsSuccess
                        ? null
                        : emailResult.ErrorMessage);

        return Results.Ok(response);
    }

    // ==================== Role Handlers ====================

    private static async Task<IResult> GetRolesAsync(
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var roles =
            await mediator.Send(
                new GetAdminRolesQuery(),
                cancellationToken);

        return Results.Ok(roles);
    }

    private static async Task<IResult> CreateRoleAsync(
        CreateAdminRoleCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var roleId =
            await mediator.Send(
                command,
                cancellationToken);

        return Results.Ok(
            new
            {
                id = roleId
            });
    }

    private static async Task<IResult> UpdateRoleAsync(
        string roleId,
        UpdateAdminRoleRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new UpdateAdminRoleCommand(
                roleId,
                request.Name,
                request.Description);

        await mediator.Send(
            command,
            cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteRoleAsync(
        string roleId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteAdminRoleCommand(roleId),
            cancellationToken);

        return Results.NoContent();
    }

    // ==================== Helpers ====================

    private static string GetCurrentUserId(
        ClaimsPrincipal principal)
    {
        var userId =
            principal.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user's identifier is missing.");
        }

        return userId;
    }

    private static string GetVerificationBaseUrl(
        IConfiguration configuration)
    {
        var apiBaseUrl =
            configuration["AppSettings:ApiBaseUrl"];

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            throw new InvalidOperationException(
                "The API base URL is not configured.");
        }

        return $"{apiBaseUrl.TrimEnd('/')}/api/auth/confirm-email";
    }

    // ==================== Requests ====================

    public sealed record UpdateAdminUserRequest(
        string Email,
        string FirstName,
        string LastName,
        string? PhoneNumber);

    public sealed record UpdateAdminUserRolesRequest(
        IReadOnlyCollection<string> Roles);

    public sealed record SetAdminUserStatusRequest(
        bool IsActive);

    public sealed record UpdateAdminRoleRequest(
        string Name,
        string? Description);

    // ==================== Responses ====================

    public sealed record CreateAdminUserResponse(
        string Id,
        bool VerificationEmailSent,
        string? VerificationEmailError);

    public sealed record VerificationEmailResponse(
        bool Sent,
        string? ErrorMessage);
}