
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Commands;
using RccgHopeHouse.Application.Features.Celebrations.Queries;

namespace RccgHopeHouse.Api.Endpoints.Celebrations;

/// <summary>
/// Defines HTTP endpoints for managing member celebrations
/// and processing secure member invitation responses.
/// </summary>
/// <remarks>
/// Administrative endpoints require the existing RequireAdmin policy.
///
/// Public invitation endpoints use secure, time-limited,
/// single-use tokens rather than authenticated member sessions.
///
/// Invitation tokens are validated by the application layer
/// before celebration information is disclosed or responses
/// are accepted.
/// </remarks>
public static class CelebrationEndpoints
{
    /// <summary>
    /// Registers administrative celebration endpoints and
    /// public invitation response endpoints.
    /// </summary>
    /// <param name="group">
    /// The application's existing API route group.
    /// </param>
    /// <returns>
    /// The configured route group.
    /// </returns>
    public static RouteGroupBuilder MapCelebrationEndpoints(
        this RouteGroupBuilder group)
    {
        ArgumentNullException.ThrowIfNull(group);

        // ====================================================
        // ROOT CELEBRATION ROUTE GROUP
        // ====================================================

        // Do not apply RequireAdmin to the root group because
        // invitation response endpoints must be accessible
        // without requiring member authentication.
        var celebrations = group.MapGroup("/celebrations")
            .WithTags("Celebrations");

        // ====================================================
        // ADMINISTRATIVE ROUTE GROUP
        // ====================================================

        // All administrative endpoints require the existing
        // RequireAdmin authorisation policy.
        var admin = celebrations.MapGroup("")
            .RequireAuthorization("RequireAdmin");

        // ====================================================
        // GET CELEBRATIONS
        // ====================================================

        // Retrieves celebrations by workflow stage
        // and optional date range.
        admin.MapGet("/", async (
            string? status,
            DateOnly? fromDate,
            DateOnly? throughDate,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetCelebrationsQuery(
                status,
                fromDate,
                throughDate);

            var result = await mediator.Send(query, ct);

            return Results.Ok(result);
        })
        .WithName("GetCelebrations")
        .WithSummary("Retrieve celebrations")
        .WithDescription(
            "Retrieves upcoming celebrations or celebrations matching a supported workflow stage.");

        // ====================================================
        // GET CELEBRATION BY ID
        // ====================================================

        // Retrieves an individual celebration.
        admin.MapGet("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new GetCelebrationByIdQuery(id),
                ct);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("GetCelebrationById")
        .WithSummary("Retrieve a celebration by ID");

        // ====================================================
        // CREATE CELEBRATION
        // ====================================================

        // Creates a new celebration.
        admin.MapPost("/", async (
            CreateCelebrationCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                command,
                ct);

            return Results.Created(
                $"/celebrations/{result.Id}",
                result);
        })
        .WithName("CreateCelebration")
        .WithSummary("Create a celebration");

        // ====================================================
        // UPDATE CELEBRATION
        // ====================================================

        // Updates the details of an existing pending celebration.
        admin.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCelebrationRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new UpdateCelebrationCommand(
                id,
                request.GalleryCategoryId,
                request.Occasion,
                request.CelebrationDate);

            var result = await mediator.Send(
                command,
                ct);

            return Results.Ok(result);
        })
        .WithName("UpdateCelebration")
        .WithSummary("Update a pending celebration");

        // ====================================================
        // DELETE CELEBRATION
        // ====================================================

        // Soft-deletes an eligible celebration.
        admin.MapDelete("/{id:guid}", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var deleted = await mediator.Send(
                new DeleteCelebrationCommand(id),
                ct);

            return deleted
                ? Results.NoContent()
                : Results.NotFound();
        })
        .WithName("DeleteCelebration")
        .WithSummary("Soft-delete a celebration");

        // ====================================================
        // ADMINISTRATOR APPROVAL
        // ====================================================

        // Records administrator approval for publication.
        admin.MapPost("/{id:guid}/approve", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new ApproveCelebrationByAdminCommand(id),
                ct);

            return Results.Ok(result);
        })
        .WithName("ApproveCelebrationByAdmin")
        .WithSummary("Approve a celebration for publication");

        // ====================================================
        // PUBLISH CELEBRATION
        // ====================================================

        // Publishes a celebration that has passed
        // administrator review.
        admin.MapPost("/{id:guid}/publish", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new PublishCelebrationCommand(id),
                ct);

            return Results.Ok(result);
        })
        .WithName("PublishCelebration")
        .WithSummary("Publish an approved celebration");

        // ====================================================
        // ISSUE CELEBRATION INVITATION
        // ====================================================

        // Sends a secure, time-limited celebration invitation
        // to the member's registered mobile number.
        //
        // This endpoint is restricted to administrators.
        //
        // The application handler generates the secure token,
        // stores its hash and sends the invitation via SMS.

        admin.MapPost("/{id:guid}/invite", async (
            Guid id,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new IssueCelebrationInvitationCommand(id),
                ct);

            return Results.Ok(new
            {
                Success = result,
                Message = "The celebration invitation has been issued."
            });
        })
        .RequireRateLimiting("Strict")
        .WithName("IssueCelebrationInvitation")
        .WithSummary("Send a celebration invitation by SMS")
        .WithDescription("Allows an administrator to send a secure, time-limited SMS invitation to a member for celebration publication consent.");

        // ====================================================
        // PUBLIC INVITATION ROUTE GROUP
        // ====================================================

        // Public invitation endpoints are intentionally
        // separated from administrator-protected routes.
        //
        // Members authenticate their invitation through
        // the secure token instead of a user account.
        var invitations = celebrations.MapGroup("/invitations")
            .WithTags("Celebration Invitations")
            .AllowAnonymous();

        // ====================================================
        // GET CELEBRATION INVITATION BY TOKEN
        // ====================================================

        // Retrieves public celebration invitation details.
        //
        // The application layer hashes and validates the token.
        // Invalid, expired, revoked or previously used tokens
        // return HTTP 404 without exposing member information.
        invitations.MapGet("/{token}", async (
            string token,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query =
                new GetCelebrationInvitationByTokenQuery(token);

            var result = await mediator.Send(
                query,
                ct);

            return result is null
                ? Results.NotFound(new
                {
                    Message =
                        "This invitation is invalid, expired or no longer available."
                })
                : Results.Ok(result);
        })
         .RequireRateLimiting("CelebrationInvitationRead")
        .WithName("GetCelebrationInvitationByToken")
        .WithSummary("Retrieve a celebration invitation")
        .WithDescription("Validates a secure invitation token and returns the celebration details required for a member response.");

        // ====================================================
        // RESPOND TO CELEBRATION INVITATION
        // ====================================================

        // Records the member's decision to approve or decline
        // publication of their celebration.
        //
        // The invitation must be valid, unexpired and unused.
        //
        // On success, the application handler updates the
        // celebration and marks the token as used.
        invitations.MapPost("/respond", async (
            RespondToCelebrationInvitationCommand command,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                command,
                ct);

            return Results.Ok(new
            {
                Success = result,
                Message = command.Approved
                    ? "Thank you. Your celebration publication consent has been recorded."
                    : "Your decision has been recorded. Your celebration will not be published."
            });
        })
        .RequireRateLimiting("CelebrationInvitationRespond")
        .WithName("RespondToCelebrationInvitation")
        .WithSummary("Respond to a celebration invitation")
        .WithDescription("Allows a member to approve or decline celebration publication using a secure, single-use invitation token.");

        return group;
    }

    /// <summary>
    /// Represents the editable details of an existing celebration.
    /// </summary>
    /// <param name="GalleryCategoryId">
    /// The existing gallery category identifier.
    /// </param>
    /// <param name="Occasion">
    /// Birthday or WeddingAnniversary.
    /// </param>
    /// <param name="CelebrationDate">
    /// The updated celebration date.
    /// </param>
    public sealed record UpdateCelebrationRequest(
        Guid GalleryCategoryId,
        string Occasion,
        DateOnly CelebrationDate);
}
