using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.GivingTypes.Commands;
using RccgHopeHouse.Application.Features.GivingTypes.Dtos;
using RccgHopeHouse.Application.Features.GivingTypes.Queries;

namespace RccgHopeHouse.Api.Endpoints.GivingTypes;

/// <summary>
/// Endpoints for managing and retrieving giving types.
///
/// Active giving types are publicly accessible so that the Give Online
/// form can populate its giving-type selection.
///
/// Administrative operations such as creating, updating, activating and
/// deactivating giving types require administrator authorization.
/// </summary>
public static class GivingTypeEndpoints
{
    public static RouteGroupBuilder MapGivingTypeEndpoints(
        this RouteGroupBuilder group)
    {
        var givingTypes = group.MapGroup("/giving-types")
                               .WithTags("Giving Types");

        // ==================== Public ====================

        givingTypes.MapGet("/active", GetActiveAsync)
                   .WithName("GetActiveGivingTypes")
                   .WithSummary("Get active giving types for the Give Online form")
                   .Produces<IReadOnlyList<GivingTypeDto>>(
                       StatusCodes.Status200OK);

        // ==================== Admin ====================

        var admin = givingTypes.MapGroup("/admin")
                               .RequireAuthorization("RequireAdmin");

        admin.MapGet("/", GetAllAsync)
             .WithName("GetGivingTypes")
             .WithSummary("Get all giving types including inactive ones")
             .Produces<IReadOnlyList<GivingTypeDto>>(
                 StatusCodes.Status200OK);

        admin.MapGet("/{id:guid}", GetByIdAsync)
             .WithName("GetGivingTypeById")
             .WithSummary("Get a giving type by ID")
             .Produces<GivingTypeDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/", CreateAsync)
             .WithName("CreateGivingType")
             .WithSummary("Create a new giving type")
             .Produces<GivingTypeDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem();

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateGivingType")
             .WithSummary("Update an existing giving type")
             .Produces<GivingTypeDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapPost("/{id:guid}/deactivate", DeactivateAsync)
             .WithName("DeactivateGivingType")
             .WithSummary("Deactivate a giving type")
             .Produces<GivingTypeDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/activate", ActivateAsync)
             .WithName("ActivateGivingType")
             .WithSummary("Reactivate a previously deactivated giving type")
             .Produces<GivingTypeDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ==================== Public Queries ====================

    /// <summary>
    /// Gets active giving types for use by the public Give Online form.
    /// </summary>
    private static async Task<IResult> GetActiveAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetActiveGivingTypesQuery();
        var list = await mediator.Send(query, ct);

        return TypedResults.Ok(list);
    }

    // ==================== Admin Queries ====================

    /// <summary>
    /// Gets all giving types, including inactive giving types.
    /// </summary>
    private static async Task<IResult> GetAllAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetAllGivingTypesQuery();
        var list = await mediator.Send(query, ct);

        return TypedResults.Ok(list);
    }

    /// <summary>
    /// Gets a single giving type by its unique identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGivingTypeByIdQuery(id);
        var givingType = await mediator.Send(query, ct);

        return TypedResults.Ok(givingType);
    }

    // ==================== Admin Commands ====================

    /// <summary>
    /// Creates a new giving type.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateGivingTypeCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetGivingTypeById",
            new { id = result.Id });
    }

    /// <summary>
    /// Updates an existing giving type.
    ///
    /// The route ID is treated as the source of truth and replaces
    /// any ID supplied in the request body.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateGivingTypeCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Deactivates a giving type without deleting its historical record.
    /// </summary>
    private static async Task<IResult> DeactivateAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeactivateGivingTypeCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Reactivates a previously deactivated giving type.
    /// </summary>
    private static async Task<IResult> ActivateAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ActivateGivingTypeCommand(id),
            ct);

        return TypedResults.Ok(result);
    }
}