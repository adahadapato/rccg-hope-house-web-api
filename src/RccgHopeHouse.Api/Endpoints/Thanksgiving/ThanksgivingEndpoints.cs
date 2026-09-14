using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Thanksgiving.Commands;
using RccgHopeHouse.Application.Features.Thanksgiving.Dtos;
using RccgHopeHouse.Application.Features.Thanksgiving.Queries;

namespace RccgHopeHouse.Api.Endpoints.Thanksgiving;

/// <summary>
/// Minimal API endpoints for Thanksgiving service videos: public monthly feed and admin sync management.
/// </summary>
public static class ThanksgivingEndpoints
{
    /// <summary>
    /// Registers all thanksgiving endpoints under the /thanksgiving route group.
    /// </summary>
    public static RouteGroupBuilder MapThanksgivingEndpoints(this RouteGroupBuilder group)
    {
        var thanks = group.MapGroup("/thanksgiving")
                          .WithTags("Thanksgiving Services")
                          .WithOpenApi();

        // ===== Public Endpoints =====
        thanks.MapGet("/", GetFeedAsync)
              .WithName("GetThanksgivingFeed")
              .WithOpenApi(x => new(x)
              {
                  Summary = "Get monthly thanksgiving service videos",
                  Description = "Returns paginated feed of YouTube-embedded videos."
              })
              .Produces<IReadOnlyList<ThanksgivingServiceDto>>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .AllowAnonymous();

        thanks.MapGet("/{year:int}/{month:int}", GetByMonthAsync)
              .WithName("GetThanksgivingByMonth")
              .WithOpenApi(x => new(x) { Summary = "Get services for specific month" })
              .Produces<IReadOnlyList<ThanksgivingServiceDto>>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = thanks.MapGroup("/admin")
                          .RequireAuthorization("RequireMediaManager");

        admin.MapPost("/", CreateAsync)
             .WithName("CreateThanksgivingService")
             .WithOpenApi(x => new(x) { Summary = "Manually add a thanksgiving service record" })
             .Produces<ThanksgivingServiceDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem();

        admin.MapPost("/sync", SyncPlaylistAsync)
             .WithName("SyncThanksgivingPlaylist")
             .WithOpenApi(x => new(x)
             {
                 Summary = "Sync videos from YouTube playlist",
                 Description = "Fetches latest videos from configured playlist and upserts to database."
             })
             .Produces<int>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        return group;
    }

    // ===== Public Handlers =====
    private static async Task<IResult> GetFeedAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetThanksgivingFeedQuery(skip, take);
        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetByMonthAsync(
        int year,
        int month,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (month < 1 || month > 12)
            return TypedResults.BadRequest("Month must be between 1 and 12.");

        var query = new GetThanksgivingFeedQuery(0, 100);
        var all = await mediator.Send(query, cancellationToken);
        var filtered = all.Where(s => s.ServiceMonth.Year == year && s.ServiceMonth.Month == month).ToList();
        return TypedResults.Ok(filtered);
    }

    // ===== Admin Handlers =====
    private static async Task<IResult> SyncPlaylistAsync(
        [FromServices] IConfiguration config,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var playlistId = config["YouTube:ThanksgivingPlaylistId"];
        if (string.IsNullOrWhiteSpace(playlistId))
            return TypedResults.BadRequest("YouTube playlist ID not configured.");

        var command = new SyncThanksgivingVideosCommand(playlistId);
        var syncedCount = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(syncedCount);
    }

    /// <summary>
    /// Handles POST /api/v1/thanksgiving/admin requests.
    /// Creates a new service record using the domain factory method via the Application command.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateThanksgivingRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // ✅ Description is now properly mapped
        var command = new CreateThanksgivingServiceCommand(
            request.Title,
            request.VideoUrl,
            request.ThumbnailUrl,
            request.ServiceMonth,
            request.Description);

        var result = await mediator.Send(command, cancellationToken);

        // ✅ Returns 201 Created with Location header pointing to the new resource
        return TypedResults.Created(
            $"/api/v1/thanksgiving/{result.ServiceMonth.Year}/{result.ServiceMonth.Month}",
            result);
    }
}

// ==================== API Layer Request DTOs ====================

/// <summary>
/// Request payload for creating a new thanksgiving service record.
/// ✅ Includes Description property for optional notes.
/// </summary>
public record CreateThanksgivingRequest(
    string Title,
    string VideoUrl,
    string ThumbnailUrl,
    DateTime ServiceMonth,
    string? Description);