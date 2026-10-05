using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

namespace RccgHopeHouse.Api.Endpoints.ServiceBroadcasts;

/// <summary>
/// Defines Minimal API endpoints for service broadcasts.
/// </summary>
public static class ServiceBroadcastEndpoints
{
    /// <summary>
    /// Maps public and administrative service broadcast endpoints.
    /// </summary>
    /// <param name="group">
    /// Parent API route group.
    /// </param>
    /// <returns>
    /// The parent route group.
    /// </returns>
    public static RouteGroupBuilder MapServiceBroadcastEndpoints(
        this RouteGroupBuilder group)
    {
        var broadcasts = group
            .MapGroup("/service-broadcasts")
            .WithTags("Service Broadcasts");

        // ===== Public Endpoints =====

        broadcasts.MapGet(
                "/latest",
                GetLatestAsync)
            .WithName(
                "GetLatestServiceBroadcasts")
            .WithSummary(
                "Get the latest published service broadcasts")
            .Produces<IReadOnlyList<ServiceBroadcastDto>>(
                StatusCodes.Status200OK)
            .AllowAnonymous();

        // ===== Admin Endpoints =====

        var admin = broadcasts
            .MapGroup("/admin")
            .RequireAuthorization(
                "RequireContentEditor");

        admin.MapGet(
                "/",
                GetAdminListAsync)
            .WithName(
                "GetServiceBroadcastsAdmin")
            .WithSummary(
                "Get service broadcast history for administration")
            .Produces<IReadOnlyList<ServiceBroadcastDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost(
                "/",
                CreateAsync)
            .WithName(
                "CreateServiceBroadcast")
            .WithSummary(
                "Record a monthly broadcast for a church service")
            .Produces<ServiceBroadcastDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest);

        admin.MapPut(
                "/{id:guid}",
                UpdateAsync)
            .WithName(
                "UpdateServiceBroadcast")
            .WithSummary(
                "Update a broadcast's video, title, description, theme or live status")
            .Produces<ServiceBroadcastDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest);

        admin.MapPut(
                "/{id:guid}/published",
                SetPublishedAsync)
            .WithName(
                "SetServiceBroadcastPublished")
            .WithSummary(
                "Publish or unpublish an individual service broadcast")
            .Produces<ServiceBroadcastDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest);

        admin.MapDelete(
                "/{id:guid}",
                DeleteAsync)
            .WithName(
                "DeleteServiceBroadcast")
            .WithSummary(
                "Delete an individual service broadcast")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public Handlers =====

    /// <summary>
    /// Gets the latest published service broadcasts.
    /// </summary>
    private static async Task<IResult> GetLatestAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result =
            await mediator.Send(
                new GetLatestServiceBroadcastsQuery(),
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    // ===== Admin Handlers =====

    /// <summary>
    /// Gets service broadcasts for administration.
    /// </summary>
    private static async Task<IResult> GetAdminListAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetServiceBroadcastsAdminQuery(
                Skip: skip,
                Take: take <= 0
                    ? 100
                    : take);

        var result =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    /// <summary>
    /// Creates a service broadcast.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateServiceBroadcastRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new CreateServiceBroadcastCommand(
                ChurchServiceId:
                    request.ChurchServiceId,
                Title:
                    request.Title,
                YoutubeUrl:
                    request.YoutubeUrl,
                ServiceMonth:
                    request.ServiceMonth,
                Description:
                    request.Description,
                Theme:
                    request.Theme,
                IsLive:
                    request.IsLive);

        var result =
            await mediator.Send(
                command,
                cancellationToken);

        return TypedResults.Created(
            $"/api/service-broadcasts/{result.Id}",
            result);
    }

    /// <summary>
    /// Updates an existing service broadcast.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody]
        UpdateServiceBroadcastCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result =
            await mediator.Send(
                command with
                {
                    Id = id
                },
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    /// <summary>
    /// Publishes or unpublishes an individual service broadcast.
    /// </summary>
    private static async Task<IResult> SetPublishedAsync(
        Guid id,
        [FromBody]
        SetServiceBroadcastPublishedRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new SetServiceBroadcastPublishedCommand(
                Id: id,
                IsPublished:
                    request.IsPublished);

        var result =
            await mediator.Send(
                command,
                cancellationToken);

        return TypedResults.Ok(
            result);
    }

    /// <summary>
    /// Deletes an individual service broadcast.
    /// </summary>
    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteServiceBroadcastCommand(
                id),
            cancellationToken);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

/// <summary>
/// Request used to create a service broadcast.
/// </summary>
public sealed record CreateServiceBroadcastRequest(
    Guid ChurchServiceId,
    string Title,
    string YoutubeUrl,
    DateTime ServiceMonth,
    string? Description = null,
    string? Theme = null,
    bool IsLive = false);

/// <summary>
/// Request used to change the public publication state
/// of an individual service broadcast.
/// </summary>
/// <param name="IsPublished">
/// <see langword="true"/> to publish the broadcast;
/// otherwise <see langword="false"/>.
/// </param>
public sealed record SetServiceBroadcastPublishedRequest(
    bool IsPublished);