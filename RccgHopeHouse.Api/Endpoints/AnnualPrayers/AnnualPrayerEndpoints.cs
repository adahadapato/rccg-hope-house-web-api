using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.AnnualPrayers.Commands;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;
using RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

namespace RccgHopeHouse.Api.Endpoints.AnnualPrayers;

/// <summary>
/// Endpoints for retrieving and managing annual prayer content.
///
/// The public endpoint provides the annual prayer currently
/// active for display on the website.
///
/// Administrative endpoints allow administrators to view,
/// create and update Annual Prayer records.
/// </summary>
public static class AnnualPrayerEndpoints
{
    public static RouteGroupBuilder MapAnnualPrayerEndpoints(
        this RouteGroupBuilder group)
    {
        var annualPrayers = group
            .MapGroup("/annual-prayers")
            .WithTags("Annual Prayers");

        // ==================== Public ====================

        annualPrayers.MapGet(
                "/active",
                GetActiveAsync)
            .WithName("GetActiveAnnualPrayer")
            .WithSummary(
                "Get the active Annual Prayer")
            .Produces<AnnualPrayerDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ==================== Admin ====================

        var admin = annualPrayers
            .MapGroup("/admin")
            .RequireAuthorization(
                "RequireAdmin");

        admin.MapGet(
                "",
                GetListAsync)
            .WithName(
                "GetAnnualPrayersList")
            .WithSummary(
                "Get all Annual Prayer records")
            .Produces<
                IReadOnlyList<AnnualPrayerDto>>(
                StatusCodes.Status200OK);

        admin.MapGet(
                "/{id:guid}",
                GetByIdAsync)
            .WithName(
                "GetAnnualPrayerById")
            .WithSummary(
                "Get an Annual Prayer by ID")
            .Produces<AnnualPrayerDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "",
                CreateAsync)
            .WithName(
                "CreateAnnualPrayer")
            .WithSummary(
                "Create a new Annual Prayer")
            .Produces<AnnualPrayerDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapPut(
                "/{id:guid}",
                UpdateAsync)
            .WithName(
                "UpdateAnnualPrayer")
            .WithSummary(
                "Update an existing Annual Prayer")
            .Produces<AnnualPrayerDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        return group;
    }

    // ==================== Public Queries ====================

    private static async Task<IResult> GetActiveAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetActiveAnnualPrayerQuery(),
            ct);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    // ==================== Admin Queries ====================

    private static async Task<IResult> GetListAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var annualPrayers =
            await mediator.Send(
                new GetAnnualPrayersListQuery(),
                ct);

        return TypedResults.Ok(
            annualPrayers);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var annualPrayer =
            await mediator.Send(
                new GetAnnualPrayerByIdQuery(
                    id),
                ct);

        if (annualPrayer is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(
            annualPrayer);
    }

    // ==================== Admin Commands ====================

    private static async Task<IResult> CreateAsync(
        [FromBody]
        CreateAnnualPrayerCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command,
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetAnnualPrayerById",
            new
            {
                id = result.Id
            });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody]
        UpdateAnnualPrayerCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with
            {
                Id = id
            },
            ct);

        return TypedResults.Ok(
            result);
    }
}