using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ChurchEvents.Commands;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Application.Features.ChurchEvents.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.ChurchEvents;

/// <summary>
/// Minimal API endpoints for church events:
/// public Upcoming Events feed and admin management.
/// </summary>
[Authorize]
public static class ChurchEventEndpoints
{
    public static RouteGroupBuilder MapChurchEventEndpoints(
        this RouteGroupBuilder group)
    {
        var events = group
            .MapGroup("/events")
            .WithTags("Church Events");

        // ===== Public Endpoints =====

        events.MapGet(
                "/upcoming",
                GetUpcomingAsync)
            .WithName("GetUpcomingChurchEvents")
            .WithSummary(
                "Get upcoming church events")
            .WithDescription(
                "Returns active church events that are currently running or scheduled for the future.")
            .Produces<
                IReadOnlyList<ChurchEventFeedDto>>(
                StatusCodes.Status200OK)
            .AllowAnonymous();

        // ===== Admin Endpoints =====

        var admin = events
            .MapGroup("/admin")
            .RequireAuthorization(
                "RequireContentEditor");

        admin.MapGet(
                "/",
                GetAdminListAsync)
            .WithName("GetChurchEvents")
            .WithSummary(
                "Get church events for administration")
            .Produces<
                IReadOnlyList<ChurchEventDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapGet(
                "/{id:guid}",
                GetByIdAsync)
            .WithName("GetChurchEventById")
            .WithSummary(
                "Get single church event by ID")
            .Produces<ChurchEventDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost(
                "/",
                CreateAsync)
            .WithName("CreateChurchEvent")
            .WithSummary(
                "Create new church event")
            .Produces<ChurchEventDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPut(
                "/{id:guid}",
                UpdateAsync)
            .WithName("UpdateChurchEvent")
            .WithSummary(
                "Update church event")
            .Produces<ChurchEventDto>(
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
            .WithName(
                "ToggleChurchEventActive")
            .WithSummary(
                "Activate or deactivate a church event")
            .Produces<ChurchEventDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapDelete(
                "/{id:guid}",
                DeleteAsync)
            .WithName(
                "DeleteChurchEvent")
            .WithSummary(
                "Permanently delete church event")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        return group;
    }

    // ===== Public Handlers =====

    private static async Task<IResult> GetUpcomingAsync(
        [FromQuery] int take = 20,
        [FromServices] IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query =
            new GetUpcomingChurchEventsQuery(
                Take: take);

        var list =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            list);
    }

    // ===== Admin Handlers =====

    private static async Task<IResult> GetAdminListAsync(
        [FromQuery] ServiceCategory? category,
        [FromQuery] bool? isActive,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromServices] IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query =
            new GetChurchEventsQuery(
                Category: category,
                IsActive: isActive,
                Skip: skip,
                Take: take);

        var list =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            list);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetChurchEventByIdQuery(
                id);

        var churchEvent =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            churchEvent);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateChurchEventRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new CreateChurchEventCommand(
                Title:
                    request.Title,
                Category:
                    request.Category,
                StartDateTime:
                    request.StartDateTime,
                EndDateTime:
                    request.EndDateTime,
                Description:
                    request.Description,
                Location:
                    request.Location,
                Icon:
                    request.Icon,
                Color:
                    request.Color,
                RegistrationUrl:
                    request.RegistrationUrl,
                RegistrationButtonText:
                    request.RegistrationButtonText,
                ImageUrl:
                    request.ImageUrl,
                DisplayOrder:
                    request.DisplayOrder,
                IsActive:
                    request.IsActive);

        var result =
            await mediator.Send(
                command,
                cancellationToken);

        return TypedResults.CreatedAtRoute(
            result,
            "GetChurchEventById",
            new
            {
                id = result.Id
            });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateChurchEventRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new UpdateChurchEventCommand(
                Id:
                    id,
                Title:
                    request.Title,
                Category:
                    request.Category,
                StartDateTime:
                    request.StartDateTime,
                EndDateTime:
                    request.EndDateTime,
                Description:
                    request.Description,
                Location:
                    request.Location,
                Icon:
                    request.Icon,
                Color:
                    request.Color,
                RegistrationUrl:
                    request.RegistrationUrl,
                RegistrationButtonText:
                    request.RegistrationButtonText,
                ImageUrl:
                    request.ImageUrl,
                DisplayOrder:
                    request.DisplayOrder,
                IsActive:
                    request.IsActive);

        var result =
            await mediator.Send(
                command,
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> ToggleActiveAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new ToggleChurchEventActiveCommand(
                id);

        var result =
            await mediator.Send(
                command,
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new DeleteChurchEventCommand(
                id);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

public record CreateChurchEventRequest(
    string Title,
    ServiceCategory Category,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    string? Description = null,
    string? Location = null,
    string? Icon = null,
    string? Color = null,
    string? RegistrationUrl = null,
    string? RegistrationButtonText = null,
    string? ImageUrl = null,
    int DisplayOrder = 0,
    bool IsActive = true);

public record UpdateChurchEventRequest(
    string Title,
    ServiceCategory Category,
    DateTime StartDateTime,
    DateTime? EndDateTime,
    string? Description,
    string? Location,
    string? Icon,
    string? Color,
    string? RegistrationUrl,
    string? RegistrationButtonText,
    string? ImageUrl,
    int DisplayOrder,
    bool IsActive);