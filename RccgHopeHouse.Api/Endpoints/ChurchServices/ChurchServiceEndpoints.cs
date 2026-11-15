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
/// monthly-service visibility, broadcast configuration,
/// current service themes and day-based filtering.
/// </summary>
[Authorize]
public static class ChurchServiceEndpoints
{
    /// <summary>
    /// Maps all public and administrative church service endpoints.
    /// </summary>
    /// <param name="group">
    /// The parent API route group.
    /// </param>
    /// <returns>
    /// The parent route group after the church service endpoints
    /// have been registered.
    /// </returns>
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

        services.MapGet("/categories", GetCategoriesAsync)
            .WithName("GetChurchServiceCategories")
            .WithSummary("Get available church service categories")
            .WithDescription(
                "Returns every category defined by the church service category enum.")
            .Produces<IReadOnlyList<string>>(
                StatusCodes.Status200OK)
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
            .WithSummary(
                "Update service schedule and display settings")
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

        admin.MapPost(
                "/{id:guid}/toggle-broadcast",
                ToggleBroadcastAsync)
            .WithName("ToggleChurchServiceBroadcast")
            .WithSummary(
                "Enable or disable broadcasting for a church service")
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

    /// <summary>
    /// Retrieves church services using the supplied public filters.
    /// Active services are returned by default unless inactive
    /// services are explicitly requested.
    /// </summary>
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

    /// <summary>
    /// Retrieves a single church service by its identifier.
    /// </summary>
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

    /// <summary>
    /// Retrieves active church services scheduled for today.
    /// </summary>
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

    /// <summary>
    /// Creates a new church service from the supplied request.
    /// </summary>
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
                    request.ShowInMonthlyServices,
                IsBroadcastEnabled:
                    request.IsBroadcastEnabled,
                CurrentTheme:
                    request.CurrentTheme);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.CreatedAtRoute(
            result,
            "GetChurchServiceById",
            new { id = result.Id });
    }

    /// <summary>
    /// Updates an existing church service from the supplied request.
    /// </summary>
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
                    request.ShowInMonthlyServices,
                IsBroadcastEnabled:
                    request.IsBroadcastEnabled,
                CurrentTheme:
                    request.CurrentTheme);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Toggles the active state of a church service.
    /// </summary>
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

    /// <summary>
    /// Toggles whether broadcasts are enabled for a church service.
    /// </summary>
    private static async Task<IResult> ToggleBroadcastAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new ToggleBroadcastCommand(id);

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Permanently deletes a church service.
    /// </summary>
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

    /// <summary>
    /// Gets every available church service category through
    /// the application CQRS pipeline.
    /// </summary>
    /// <param name="mediator">
    /// Mediator used to dispatch the service categories query.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// An HTTP 200 response containing all church service
    /// category names.
    /// </returns>
    private static async Task<IResult> GetCategoriesAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetChruchServiceCategoriesQuery();

        var categories =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(categories);
    }
}

// ==================== API Request DTOs ====================

/// <summary>
/// Request model used when creating a new church service.
/// </summary>
/// <param name="Name">
/// The name of the church service.
/// </param>
/// <param name="Category">
/// The category to which the service belongs.
/// </param>
/// <param name="DayOfWeek">
/// The normal day of the week for the service.
/// </param>
/// <param name="StartTime">
/// The scheduled start time, if specified.
/// </param>
/// <param name="EndTime">
/// The scheduled end time, if specified.
/// </param>
/// <param name="Description">
/// An optional description of the service.
/// </param>
/// <param name="Location">
/// The physical service location, if applicable.
/// </param>
/// <param name="ZoomId">
/// The Zoom meeting identifier, if applicable.
/// </param>
/// <param name="ZoomPasscode">
/// The Zoom meeting passcode, if applicable.
/// </param>
/// <param name="Recurrence">
/// Defines how frequently the service occurs.
/// </param>
/// <param name="DayOfMonth">
/// The configured day of the month for monthly services.
/// </param>
/// <param name="IsLocal">
/// Indicates whether the service belongs to Hope House locally.
/// </param>
/// <param name="DisplayOrder">
/// Determines the display position of the service.
/// </param>
/// <param name="Icon">
/// Optional presentation icon for the service.
/// </param>
/// <param name="ShowInMonthlyServices">
/// Indicates whether the service appears in the public
/// Special Monthly Services section.
/// </param>
/// <param name="IsBroadcastEnabled">
/// Indicates whether broadcasts may be associated with the service.
/// </param>
/// <param name="CurrentTheme">
/// The theme for the upcoming or currently occurring
/// instance of the service.
/// </param>
public sealed record CreateChurchServiceRequest(
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
    bool ShowInMonthlyServices = false,
    bool IsBroadcastEnabled = false,
    string? CurrentTheme = null);

/// <summary>
/// Request model used when updating an existing church service.
/// </summary>
/// <param name="Name">
/// The name of the church service.
/// </param>
/// <param name="Category">
/// The category to which the service belongs.
/// </param>
/// <param name="DayOfWeek">
/// The normal day of the week for the service.
/// </param>
/// <param name="StartTime">
/// The scheduled start time, if specified.
/// </param>
/// <param name="EndTime">
/// The scheduled end time, if specified.
/// </param>
/// <param name="Description">
/// An optional description of the service.
/// </param>
/// <param name="Location">
/// The physical service location, if applicable.
/// </param>
/// <param name="ZoomId">
/// The Zoom meeting identifier, if applicable.
/// </param>
/// <param name="ZoomPasscode">
/// The Zoom meeting passcode, if applicable.
/// </param>
/// <param name="Recurrence">
/// Defines how frequently the service occurs.
/// </param>
/// <param name="DayOfMonth">
/// The configured day of the month for monthly services.
/// </param>
/// <param name="IsLocal">
/// Indicates whether the service belongs to Hope House locally.
/// </param>
/// <param name="DisplayOrder">
/// Determines the display position of the service.
/// </param>
/// <param name="Icon">
/// Optional presentation icon for the service.
/// </param>
/// <param name="ShowInMonthlyServices">
/// Indicates whether the service appears in the public
/// Special Monthly Services section.
/// </param>
/// <param name="IsBroadcastEnabled">
/// Indicates whether broadcasts may be associated with the service.
/// </param>
/// <param name="CurrentTheme">
/// The theme for the upcoming or currently occurring
/// instance of the service.
/// </param>
public sealed record UpdateChurchServiceRequest(
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
    bool ShowInMonthlyServices,
    bool IsBroadcastEnabled,
    string? CurrentTheme);