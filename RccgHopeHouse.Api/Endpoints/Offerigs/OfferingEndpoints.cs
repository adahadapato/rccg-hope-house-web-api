using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Offerings.Commands;
using RccgHopeHouse.Application.Features.Offerings.Dtos;
using RccgHopeHouse.Application.Features.Offerings.Queries;

namespace RccgHopeHouse.Api.Endpoints.Offerings;

/// <summary>
/// Endpoints for submitting and managing online offerings.
///
/// Creating an offering is publicly accessible so visitors can use the
/// Give Online feature. Access to offering records is restricted to
/// administrators because records may contain personal and financial data.
/// </summary>
public static class OfferingEndpoints
{
    public static RouteGroupBuilder MapOfferingEndpoints(
        this RouteGroupBuilder group)
    {
        var offerings = group.MapGroup("/offerings")
                             .WithTags("Offerings");

        // ==================== Public ====================

        offerings.MapPost("/", CreateAsync)
                 .WithName("CreateOffering")
                 .WithSummary("Submit a new online offering")
                 .Produces<OfferingDto>(StatusCodes.Status201Created)
                 .ProducesValidationProblem()
                 .ProducesProblem(StatusCodes.Status404NotFound);

        // ==================== Admin ====================

        var admin = offerings.MapGroup("/admin")
                             .RequireAuthorization("RequireAdmin");

        admin.MapGet("/", GetListAsync)
             .WithName("GetOfferings")
             .WithSummary("Get paginated offering records")
             .Produces<IReadOnlyList<OfferingDto>>(
                 StatusCodes.Status200OK);

        admin.MapGet("/{id:guid}", GetByIdAsync)
             .WithName("GetOfferingById")
             .WithSummary("Get a single offering by ID")
             .Produces<OfferingDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    /// <summary>
    /// Creates a new offering in the Pending payment state.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateOfferingCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.Created(string.Empty, result);
    }

    /// <summary>
    /// Gets a paginated list of offerings for administration.
    /// </summary>
    private static async Task<IResult> GetListAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        if (skip < 0)
            skip = 0;

        if (take <= 0 || take > 100)
            take = 100;

        var query = new GetOfferingsQuery(skip, take);
        var list = await mediator.Send(query, ct);

        return TypedResults.Ok(list);
    }

    /// <summary>
    /// Gets a single offering by its unique identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetOfferingByIdQuery(id);
        var offering = await mediator.Send(query, ct);

        return TypedResults.Ok(offering);
    }
}