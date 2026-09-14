using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Sermons.Commands;
using RccgHopeHouse.Application.Features.Sermons.Dtos;

using RccgHopeHouse.Application.Features.Sermons.Queries;

namespace RccgHopeHouse.Api.Endpoints.Sermons;

/// <summary>
/// Minimal API endpoints for Sermons: public feed and admin management.
/// </summary>
public static class SermonEndpoints
{
    /// <summary>
    /// Registers all sermon endpoints under the /sermons route group.
    /// </summary>
    public static RouteGroupBuilder MapSermonEndpoints(this RouteGroupBuilder group)
    {
        var sermons = group.MapGroup("/sermons")
                           .WithTags("Sermons")
                           .WithOpenApi();

        // ===== Public Endpoints =====
        sermons.MapGet("/", GetListAsync)
               .WithName("GetSermons")
               .WithOpenApi(x => new(x)
               {
                   Summary = "Get paginated list of sermons",
                   Description = "Supports search by title/speaker and date filtering."
               })
               .Produces<IReadOnlyList<SermonDto>>(StatusCodes.Status200OK)
               .AllowAnonymous();

        sermons.MapGet("/{id:guid}", GetByIdAsync)
               .WithName("GetSermonById")
               .WithOpenApi(x => new(x) { Summary = "Get sermon details by ID" })
               .Produces<SermonDto>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = sermons.MapGroup("/admin")
                           .RequireAuthorization("RequireMediaManager");

        // ✅ This maps to the CreateSermonAsync method defined below
        admin.MapPost("/", CreateSermonAsync)
             .WithName("CreateSermon")
             .WithOpenApi(x => new(x) { Summary = "Create a new sermon record" })
             .Produces<SermonDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem();

        admin.MapPut("/{id:guid}", UpdateSermonAsync)
             .WithName("UpdateSermon")
             .WithOpenApi(x => new(x) { Summary = "Update existing sermon details" })
             .Produces<SermonDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapDelete("/{id:guid}", DeleteSermonAsync)
             .WithName("DeleteSermon")
             .WithOpenApi(x => new(x) { Summary = "Delete a sermon record" })
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public Handlers =====

    private static async Task<IResult> GetListAsync(
        [FromQuery] string? search,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetSermonsQuery(search, fromDate, toDate, skip, take);
        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetSermonByIdQuery(id);
        var sermon = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(sermon);
    }

    // ===== Admin Handlers =====

    /// <summary>
    /// Handles POST /api/v1/sermons/admin requests.
    /// Creates a new sermon using the domain factory via Application command.
    /// </summary>
    private static async Task<IResult> CreateSermonAsync(
        [FromBody] CreateSermonRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateSermonCommand(
            request.Title,
            request.Speaker,
            request.ServiceDate,
            request.VideoUrl,
            request.Description);

        var result = await mediator.Send(command, cancellationToken);

        // Returns 201 Created with Location header
        return TypedResults.Created(
            $"/api/v1/sermons/{result.Id}",
            result);
    }

    private static async Task<IResult> UpdateSermonAsync(
        Guid id,
        [FromBody] UpdateSermonRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSermonCommand(
            id,
            request.Title,
            request.Speaker,
            request.ServiceDate,
            request.VideoUrl,
            request.Description);

        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteSermonAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSermonCommand(id);
        await mediator.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

// ==================== API Layer Request DTOs ====================

/// <summary>
/// Request payload for creating a new sermon.
/// </summary>
public record CreateSermonRequest(
    string Title,
    string Speaker,
    DateTime ServiceDate,
    string VideoUrl,
    string? Description);

/// <summary>
/// Request payload for updating an existing sermon.
/// </summary>
public record UpdateSermonRequest(
    string Title,
    string Speaker,
    DateTime ServiceDate,
    string VideoUrl,
    string? Description);