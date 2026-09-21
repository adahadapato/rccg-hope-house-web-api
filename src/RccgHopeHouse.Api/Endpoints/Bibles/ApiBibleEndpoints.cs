using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Bibles.Dtos;
using RccgHopeHouse.Application.Features.Bibles.Queries;

namespace RccgHopeHouse.Api.Endpoints.Bibles;

/// <summary>
/// Minimal API endpoints for Bible-related functionality.
/// </summary>
public static class ApiBibleEndpoints
{
    public static RouteGroupBuilder MapApiBibleEndpoints(
        this RouteGroupBuilder group)
    {
        var bible = group.MapGroup("/bible")
                         .WithTags("Bible");

        bible.MapGet("/bibles", GetAvailableBiblesAsync)
             .WithName("GetAvailableBibles")
             .WithSummary("Get available Bible translations")
             .WithDescription(
                 "Returns the English Bible translations available through the configured API.Bible integration.")
             .Produces<IReadOnlyList<ApiBibleTranslationDto>>(
                 StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status500InternalServerError)
             .AllowAnonymous();

        bible.MapGet("/passage", GetPassageAsync)
            .WithName("GetBiblePassage")
            .WithSummary("Get a Bible passage")
            .WithDescription(
                "Returns scripture text from a specified API.Bible translation and passage.")
            .Produces<BiblePassageDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AllowAnonymous();

        return group;
    }

    private static async Task<IResult> GetAvailableBiblesAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailableBiblesQuery();

        var translations =
            await mediator.Send(query, cancellationToken);

        return TypedResults.Ok(translations);
    }

    private static async Task<IResult> GetPassageAsync(
    [FromQuery] string bibleId,
    [FromQuery] string passageId,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(bibleId) ||
            string.IsNullOrWhiteSpace(passageId))
        {
            return TypedResults.BadRequest();
        }

        var query = new GetBiblePassageQuery(
            BibleId: bibleId,
            PassageId: passageId);

        var passage = await mediator.Send(
            query,
            cancellationToken);

        return TypedResults.Ok(passage);
    }
}