using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.PrayerRequests.Commands;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Application.Features.PrayerRequests.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.PrayerRequests;

/// <summary>
/// Minimal API endpoints for prayer request submission and pastoral management.
/// Public endpoint for submission; admin endpoints for workflow tracking.
/// </summary>
public static class PrayerRequestEndpoints
{
    public static RouteGroupBuilder MapPrayerRequestEndpoints(this RouteGroupBuilder group)
    {
        var prayers = group.MapGroup("/prayer-requests")
                           .WithTags("Prayer Requests")
                           .WithOpenApi();

        // ===== Public Endpoint =====
        prayers.MapPost("/", SubmitAsync)
               .WithName("SubmitPrayerRequest")
               .WithOpenApi(x => new(x)
               {
                   Summary = "Submit a new prayer request",
                   Description = "Public form submission. Returns 204 No Content on success; pastoral team is notified asynchronously."
               })
               .RequireRateLimiting("Strict") // ← Prevent spam submissions
               .Produces(StatusCodes.Status204NoContent)
               .ProducesValidationProblem()
               .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = prayers.MapGroup("/admin")
                           .RequireAuthorization("RequirePrayerTeam");

        admin.MapGet("/", GetListAsync)
             .WithName("GetPrayerRequests")
             .WithOpenApi(x => new(x) { Summary = "Get paginated prayer requests for pastoral dashboard" })
             .Produces<IReadOnlyList<PrayerRequestDto>>()
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapGet("/stats", GetStatsAsync)
             .WithName("GetPrayerRequestStats")
             .WithOpenApi(x => new(x) { Summary = "Get dashboard statistics (counts by status)" })
             .Produces<PrayerRequestStatsDto>()
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapGet("/{id:guid}", GetByIdAsync)
             .WithName("GetPrayerRequestById")
             .WithOpenApi(x => new(x) { Summary = "Get single request for pastoral review" })
             .Produces<PrayerRequestDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPut("/{id:guid}/status", UpdateStatusAsync)
             .WithName("UpdatePrayerRequestStatus")
             .WithOpenApi(x => new(x) { Summary = "Update pastoral workflow status" })
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        return group;
    }

    // ===== Public Handler =====
    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitPrayerRequestRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new SubmitPrayerRequestCommand(
            request.RequesterName,
            request.Content,
            request.RequesterEmail,
            request.RequesterPhone);

        await mediator.Send(command, ct);
        return TypedResults.NoContent(); // Don't expose internal IDs publicly
    }

    // ===== Admin Handlers =====
    private static async Task<IResult> GetListAsync(
        [FromQuery] PrayerRequestStatus? status,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPrayerRequestsQuery(status, skip, take);
        var requests = await mediator.Send(query, ct);
        return TypedResults.Ok(requests);
    }

    private static async Task<IResult> GetStatsAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetPrayerRequestStatsQuery();
        var stats = await mediator.Send(query, ct);
        return TypedResults.Ok(stats);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPrayerRequestByIdQuery(id);
        var request = await mediator.Send(query, ct);
        return TypedResults.Ok(request);
    }

    private static async Task<IResult> UpdateStatusAsync(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdatePrayerRequestStatusCommand(id, request.NewStatus, request.PastoralNote);
        await mediator.Send(command, ct);
        return TypedResults.NoContent();
    }
}

// ==================== API Layer Request DTOs ====================
public record SubmitPrayerRequestRequest(
    string RequesterName,
    string Content,
    string? RequesterEmail,
    string? RequesterPhone);

public record UpdateStatusRequest(PrayerRequestStatus NewStatus, string? PastoralNote);