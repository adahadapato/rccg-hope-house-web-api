using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Application.Features.ChurchServices.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.ChurchServices;

/// <summary>
/// Minimal API endpoints for church service schedules: public timetable and admin management.
/// Supports recurrence patterns, Zoom integration, and day-based filtering.
/// </summary>
[Authorize] // Default policy; override with [AllowAnonymous] on public endpoints
public static class ChurchServiceEndpoints
{
    /// <summary>
    /// Registers all church service endpoints under the /services route group.
    /// Public endpoints allow anonymous access; admin endpoints require ContentEditor role.
    /// </summary>
    public static RouteGroupBuilder MapChurchServiceEndpoints(this RouteGroupBuilder group)
    {
        var services = group.MapGroup("/services")
                            .WithTags("Church Services")
                            .WithOpenApi();

        // ===== Public Endpoints =====
        services.MapGet("/", GetListAsync)
                .WithName("GetChurchServices")
                .WithOpenApi(x => new(x)
                {
                    Summary = "Get church service schedule",
                    Description = "Returns active services, optionally filtered by category or day of week."
                })
                .Produces<IReadOnlyList<ChurchServiceFeedDto>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();

        services.MapGet("/today", GetTodayAsync)
                .WithName("GetTodayServices")
                .WithOpenApi(x => new(x) { Summary = "Get today's service schedule" })
                .Produces<IReadOnlyList<ChurchServiceFeedDto>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = services.MapGroup("/admin")
                            .RequireAuthorization("RequireContentEditor");

        admin.MapPost("/", CreateAsync)
             .WithName("CreateChurchService")
             .WithOpenApi(x => new(x) { Summary = "Create new service schedule" })
             .Produces<ChurchServiceDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateChurchService")
             .WithOpenApi(x => new(x) { Summary = "Update service time, location, or Zoom details" })
             .Produces<ChurchServiceDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/toggle-active", ToggleActiveAsync)
             .WithName("ToggleChurchServiceActive")
             .WithOpenApi(x => new(x) { Summary = "Activate/deactivate service (hide from public)" })
             .Produces<ChurchServiceDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapDelete("/{id:guid}", DeleteAsync)
             .WithName("DeleteChurchService")
             .WithOpenApi(x => new(x) { Summary = "Permanently delete service schedule" })
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    // ===== Public Handlers =====

    private static async Task<IResult> GetListAsync(
        [FromQuery] ServiceCategory? category,
        [FromQuery] DayOfWeek? dayOfWeek,
        [FromQuery] bool includeInactive,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // ✅ Use matching named arguments exactly as defined in the record
        var query = new GetChurchServicesQuery(
            Category: category,
            DayOfWeek: dayOfWeek,
            IsActive: includeInactive);

        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetTodayAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var today = DateTime.Today.DayOfWeek;

        // ✅ Match parameter name exactly
        var query = new GetChurchServicesQuery(
            DayOfWeek: today,
            IsActive: false);

        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }

    // ===== Admin Handlers =====

    /// <summary>
    /// Handles POST /api/v1/services/admin requests.
    /// Creates a new church service schedule (defaults to active).
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateChurchServiceRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateChurchServiceCommand(
            request.Name,
            request.Category,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Location,
            request.ZoomId,
            request.ZoomPasscode,
            request.Recurrence,
            request.DayOfMonth,
            request.DisplayOrder);

        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.CreatedAtRoute(result, "GetChurchServiceById", new { id = result.Id });
    }

    /// <summary>
    /// Handles PUT /api/v1/services/admin/{id} requests.
    /// Updates an existing service schedule's metadata and configuration.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateChurchServiceRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChurchServiceCommand(
            id,
            request.Name,
            request.Category,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Description,
            request.Location,
            request.ZoomId,
            request.ZoomPasscode,
            request.Recurrence,
            request.DayOfMonth,
            request.DisplayOrder);

        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Handles POST /api/v1/services/admin/{id}/toggle-active requests.
    /// Toggles a service's active status (hides/shows from public feed).
    /// </summary>
    private static async Task<IResult> ToggleActiveAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ToggleActiveCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Handles DELETE /api/v1/services/admin/{id} requests.
    /// Permanently removes a service schedule from the database.
    /// </summary>
    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteChurchServiceCommand(id);
        await mediator.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

// ==================== API Layer Request DTOs ====================

/// <summary>
/// Request payload for creating a new church service.
/// Maps to Application.CreateChurchServiceCommand.
/// </summary>
public record CreateChurchServiceRequest(
    string Name,
    ServiceCategory Category,
    DayOfWeek DayOfWeek,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Description,
    string? Location,
    string? ZoomId,
    string? ZoomPasscode,
    RecurrencePattern Recurrence,
    int? DayOfMonth,
    int DisplayOrder = 0);

/// <summary>
/// Request payload for updating an existing church service.
/// Maps to Application.UpdateChurchServiceCommand.
/// </summary>
public record UpdateChurchServiceRequest(
    string Name,
    ServiceCategory Category,
    DayOfWeek DayOfWeek,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Description,
    string? Location,
    string? ZoomId,
    string? ZoomPasscode,
    RecurrencePattern Recurrence,
    int? DayOfMonth,
    int DisplayOrder);