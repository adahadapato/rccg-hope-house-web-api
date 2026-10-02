using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.PrayerRequests.Commands;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Application.Features.PrayerRequests.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.PrayerRequests;

/// <summary>
/// Defines Minimal API endpoints for public prayer request submission
/// and authorised pastoral management.
/// </summary>
public static class PrayerRequestEndpoints
{
    /// <summary>
    /// Maps the Prayer Request endpoints to the supplied API route group.
    /// </summary>
    /// <param name="group">
    /// The parent API route group.
    /// </param>
    /// <returns>
    /// The original parent route group.
    /// </returns>
    public static RouteGroupBuilder MapPrayerRequestEndpoints(
        this RouteGroupBuilder group)
    {
        var prayers =
            group
                .MapGroup("/prayer-requests")
                .WithTags("Prayer Requests");

        // ============================================================
        // Public endpoint
        // ============================================================

        prayers
            .MapPost("/", SubmitAsync)
            .WithName("SubmitPrayerRequest")
            .WithSummary("Submit a new prayer request")
            .WithDescription(
                "Accepts a public prayer request submission. " +
                "The request is saved before email notifications " +
                "are attempted.")
            .RequireRateLimiting("Strict")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .AllowAnonymous();

        // ============================================================
        // Prayer team endpoints
        // ============================================================

        var admin =
            prayers
                .MapGroup("/admin")
                .RequireAuthorization("RequirePrayerTeam");

        admin
            .MapGet("/", GetListAsync)
            .WithName("GetPrayerRequests")
            .WithSummary(
                "Get paginated prayer requests for pastoral management")
            .Produces<IReadOnlyList<PrayerRequestDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin
            .MapGet("/stats", GetStatsAsync)
            .WithName("GetPrayerRequestStats")
            .WithSummary(
                "Get prayer request counts by workflow status")
            .Produces<PrayerRequestStatsDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin
            .MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetPrayerRequestById")
            .WithSummary(
                "Get a prayer request for pastoral review")
            .Produces<PrayerRequestDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin
            .MapPut("/{id:guid}/status", UpdateStatusAsync)
            .WithName("UpdatePrayerRequestStatus")
            .WithSummary(
                "Update prayer request workflow status and pastoral note")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        admin
            .MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeletePrayerRequest")
            .WithSummary(
                "Permanently delete a prayer request")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        return group;
    }

    // ================================================================
    // Public handler
    // ================================================================

    /// <summary>
    /// Submits a new prayer request.
    /// </summary>
    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitPrayerRequestRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command =
            new SubmitPrayerRequestCommand(
                Content: request.Content,
                IsAnonymous: request.IsAnonymous,
                RequesterName: request.RequesterName,
                PhoneNumber: request.RequesterPhone,
                RequesterEmail: request.RequesterEmail);

        await mediator.Send(
            command,
            ct);

        return TypedResults.NoContent();
    }

    // ================================================================
    // Prayer team handlers
    // ================================================================

    /// <summary>
    /// Retrieves prayer requests using optional status filtering
    /// and pagination.
    /// </summary>
    private static async Task<IResult> GetListAsync(
        [FromQuery] PrayerRequestStatus? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        IMediator mediator = null!,
        CancellationToken ct = default)
    {
        var query =
            new GetPrayerRequestsQuery(
                Status: status,
                Skip: skip,
                Take: take);

        var requests =
            await mediator.Send(
                query,
                ct);

        return TypedResults.Ok(
            requests);
    }

    /// <summary>
    /// Retrieves prayer request workflow statistics.
    /// </summary>
    private static async Task<IResult> GetStatsAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query =
            new GetPrayerRequestStatsQuery();

        var stats =
            await mediator.Send(
                query,
                ct);

        return TypedResults.Ok(
            stats);
    }

    /// <summary>
    /// Retrieves a single prayer request by identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query =
            new GetPrayerRequestByIdQuery(
                id);

        var request =
            await mediator.Send(
                query,
                ct);

        return TypedResults.Ok(
            request);
    }

    /// <summary>
    /// Updates the pastoral workflow status and optional
    /// pastoral note for a prayer request.
    /// </summary>
    private static async Task<IResult> UpdateStatusAsync(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command =
            new UpdatePrayerRequestStatusCommand(
                Id: id,
                NewStatus: request.NewStatus,
                PastoralNote: request.PastoralNote);

        await mediator.Send(
            command,
            ct);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Permanently deletes a prayer request.
    /// </summary>
    private static async Task<IResult> DeleteAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command =
            new DeletePrayerRequestCommand(
                Id: id);

        await mediator.Send(
            command,
            ct);

        return TypedResults.NoContent();
    }
}

/// <summary>
/// Request model used by the public prayer request submission endpoint.
/// </summary>
/// <param name="Content">
/// The prayer request content.
/// </param>
/// <param name="IsAnonymous">
/// Indicates whether the requester wishes to remain anonymous.
/// </param>
/// <param name="RequesterName">
/// The requester's name when the request is not anonymous.
/// </param>
/// <param name="RequesterEmail">
/// The requester's optional email address.
/// </param>
/// <param name="RequesterPhone">
/// The requester's optional phone number.
/// </param>
public record SubmitPrayerRequestRequest(
    string Content,
    bool IsAnonymous,
    string? RequesterName,
    string? RequesterEmail,
    string? RequesterPhone);

/// <summary>
/// Request model used to update the pastoral workflow state
/// of a prayer request.
/// </summary>
/// <param name="NewStatus">
/// The new workflow status.
/// </param>
/// <param name="PastoralNote">
/// An optional internal pastoral note.
/// </param>
public record UpdateStatusRequest(
    PrayerRequestStatus NewStatus,
    string? PastoralNote);