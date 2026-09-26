using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

namespace RccgHopeHouse.Api.Endpoints.ServiceBroadcasts;

public static class ServiceBroadcastEndpoints
{
    public static RouteGroupBuilder MapServiceBroadcastEndpoints(
        this RouteGroupBuilder group)
    {
        var broadcasts = group
            .MapGroup("/service-broadcasts")
            .WithTags("Service Broadcasts");

        // ===== Public Endpoints =====

        broadcasts.MapGet("/latest", GetLatestAsync)
            .WithName("GetLatestServiceBroadcasts")
            .WithSummary(
                "Get the latest configured service broadcasts")
            .Produces<IReadOnlyList<ServiceBroadcastDto>>(
                StatusCodes.Status200OK)
            .AllowAnonymous();

        // ===== Admin Endpoints =====

        var admin = broadcasts
            .MapGroup("/admin")
            .RequireAuthorization(
                "RequireContentEditor");

        admin.MapGet("/", GetAdminListAsync)
            .WithName("GetServiceBroadcastsAdmin")
            .WithSummary(
                "Get service broadcast history for administration")
            .Produces<IReadOnlyList<ServiceBroadcastDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost("/", CreateAsync)
            .WithName("CreateServiceBroadcast")
            .WithSummary(
                "Record a monthly broadcast for a church service")
            .Produces<ServiceBroadcastDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateServiceBroadcast")
            .WithSummary(
                "Update a broadcast's video, title, description, theme or live status")
            .Produces<ServiceBroadcastDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem(
                StatusCodes.Status400BadRequest);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteServiceBroadcast")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public Handlers =====

    private static async Task<IResult> GetLatestAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetLatestServiceBroadcastsQuery(),
            cancellationToken);

        return TypedResults.Ok(result);
    }

    // ===== Admin Handlers =====

    private static async Task<IResult> GetAdminListAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetServiceBroadcastsAdminQuery(
                Skip: skip,
                Take: take <= 0 ? 100 : take);

        var result = await mediator.Send(
            query,
            cancellationToken);

        return TypedResults.Ok(result);
    }

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

        var result = await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.Created(
            $"/api/service-broadcasts/{result.Id}",
            result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody]
        UpdateServiceBroadcastCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            command with { Id = id },
            cancellationToken);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteServiceBroadcastCommand(id),
            cancellationToken);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

public record CreateServiceBroadcastRequest(
    Guid ChurchServiceId,
    string Title,
    string YoutubeUrl,
    DateTime ServiceMonth,
    string? Description = null,
    string? Theme = null,
    bool IsLive = false);