using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ChurchServices.Commands;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Application.Features.ChurchServices.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.ChurchServices;

/// <summary>
/// Minimal API endpoints for church service schedules:
/// public timetable and admin management.
/// Supports recurrence patterns, Zoom integration,
/// monthly-service visibility and day-based filtering.
/// </summary>
[Authorize]
public static class ChurchServiceEndpoints
{
    public static RouteGroupBuilder MapChurchServiceEndpoints(
        this RouteGroupBuilder group)
    {
        var services = group
            .MapGroup("/services")
            .WithTags("Church Services");

        // ===== Public Endpoints =====

        services.MapGet("/", GetListAsync)
            .WithName("GetChurchServices")
            .WithSummary("Get church service schedule")
            .WithDescription(
                "Returns active services by default, optionally filtered by category, day of week, or local vs. HQ-broadcast.")
            .Produces<IReadOnlyList<ChurchServiceFeedDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        services.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetChurchServiceById")
            .WithSummary("Get single service by ID")
            .Produces<ChurchServiceDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        services.MapGet("/today", GetTodayAsync)
            .WithName("GetTodayServices")
            .WithSummary("Get today's service schedule")
            .Produces<IReadOnlyList<ChurchServiceFeedDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        // ===== Admin Endpoints =====

        var admin = services
            .MapGroup("/admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapPost("/", CreateAsync)
            .WithName("CreateChurchService")
            .WithSummary("Create new service schedule")
            .Produces<ChurchServiceDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateChurchService")
            .WithSummary("Update service schedule and display settings")
            .Produces<ChurchServiceDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost(
                "/{id:guid}/toggle-active",
                ToggleActiveAsync)
            .WithName("ToggleChurchServiceActive")
            .WithSummary(
                "Activate or deactivate a church service")
            .Produces<ChurchServiceDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteChurchService")
            .WithSummary(
                "Permanently delete service schedule")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        return group;
    }

    // ===== Public Handlers =====

    private static async Task<IResult> GetListAsync(
        [FromQuery] ServiceCategory? category,
        [FromQuery] DayOfWeek? dayOfWeek,
        [FromQuery] bool? isLocal,
        [FromQuery] bool includeInactive = false,
        [FromServices] IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new GetChurchServicesQuery(
            Category: category,
            DayOfWeek: dayOfWeek,
            IsActive: includeInactive ? null : true,
            IsLocal: isLocal);

        var list = await mediator.Send(
            query,
            cancellationToken);

        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetChurchServiceByIdQuery(id);

        var service = await mediator.Send(
            query,
            cancellationToken);

        return TypedResults.Ok(service);
    }

    private static async Task<IResult> GetTodayAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var today =
            DateTime.Today.DayOfWeek;

        var query = new GetChurchServicesQuery(
            DayOfWeek: today,
            IsActive: true);

        var list = await mediator.Send(
            query,
            cancellationToken);

        return TypedResults.Ok(list);
    }

    // ===== Admin Handlers =====

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateChurchServiceRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new CreateChurchServiceCommand(
                Name: request.Name,
                Category: request.Category,
                DayOfWeek: request.DayOfWeek,
                StartTime: request.StartTime,
                EndTime: request.EndTime,
                Description: request.Description,
                Location: request.Location,
                ZoomId: request.ZoomId,
                ZoomPasscode: request.ZoomPasscode,
                Recurrence: request.Recurrence,
                DayOfMonth: request.DayOfMonth,
                IsLocal: request.IsLocal,
                DisplayOrder: request.DisplayOrder,
                Icon: request.Icon,
                ShowInMonthlyServices:
                    request.ShowInMonthlyServices);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.CreatedAtRoute(
            result,
            "GetChurchServiceById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateChurchServiceRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new UpdateChurchServiceCommand(
                Id: id,
                Name: request.Name,
                Category: request.Category,
                DayOfWeek: request.DayOfWeek,
                StartTime: request.StartTime,
                EndTime: request.EndTime,
                Description: request.Description,
                Location: request.Location,
                ZoomId: request.ZoomId,
                ZoomPasscode: request.ZoomPasscode,
                Recurrence: request.Recurrence,
                DayOfMonth: request.DayOfMonth,
                IsLocal: request.IsLocal,
                DisplayOrder: request.DisplayOrder,
                Icon: request.Icon,
                ShowInMonthlyServices:
                    request.ShowInMonthlyServices);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ToggleActiveAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new ToggleActiveCommand(id);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new DeleteChurchServiceCommand(id);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

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
    bool IsLocal = true,
    int DisplayOrder = 0,
    string? Icon = null,
    bool ShowInMonthlyServices = false);

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
    bool IsLocal,
    int DisplayOrder,
    string? Icon,
    bool ShowInMonthlyServices);