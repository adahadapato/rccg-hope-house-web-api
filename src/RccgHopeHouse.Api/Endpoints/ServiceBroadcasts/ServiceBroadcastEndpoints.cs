using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

namespace RccgHopeHouse.Api.Endpoints.ServiceBroadcasts;

public static class ServiceBroadcastEndpoints
{
    public static RouteGroupBuilder MapServiceBroadcastEndpoints(this RouteGroupBuilder group)
    {
        var broadcasts = group.MapGroup("/service-broadcasts")
                              .WithTags("Service Broadcasts");

        broadcasts.MapGet("/latest", GetLatestAsync)
                  .WithName("GetLatestServiceBroadcasts")
                  .WithSummary("Get this month's Holy Communion, Holy Ghost Service, and Thanksgiving videos")
                  .Produces<IReadOnlyList<ServiceBroadcastDto>>(StatusCodes.Status200OK)
                  .AllowAnonymous();

        var admin = broadcasts.MapGroup("/admin")
                              .RequireAuthorization("RequireContentEditor");

        admin.MapPost("/", CreateAsync)
             .WithName("CreateServiceBroadcast")
             .WithSummary("Record this month's broadcast video for a service")
             .Produces<ServiceBroadcastDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem();

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateServiceBroadcast")
             .WithSummary("Update a broadcast's video/title/description")
             .Produces<ServiceBroadcastDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapDelete("/{id:guid}", DeleteAsync)
             .WithName("DeleteServiceBroadcast")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> GetLatestAsync(IMediator mediator, CancellationToken ct)
    {
        var result = await mediator.Send(new GetLatestServiceBroadcastsQuery(), ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateServiceBroadcastCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return TypedResults.Created($"/api/service-broadcasts/{result.Id}", result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateServiceBroadcastCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command with { Id = id }, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        await mediator.Send(new DeleteServiceBroadcastCommand(id), ct);
        return TypedResults.NoContent();
    }
}