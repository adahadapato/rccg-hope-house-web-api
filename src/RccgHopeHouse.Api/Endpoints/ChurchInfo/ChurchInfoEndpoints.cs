using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ChurchInfo.Commands;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Application.Features.ChurchInfo.Queries;

namespace RccgHopeHouse.Api.Endpoints.ChurchInfo;

/// <summary>
/// Minimal API endpoints for the church's contact/profile info: public
/// read for the Contact page, admin-only updates.
/// </summary>
public static class ChurchInfoEndpoints
{
    public static RouteGroupBuilder MapChurchInfoEndpoints(this RouteGroupBuilder group)
    {
        var churchInfo = group.MapGroup("/church-info")
                              .WithTags("Church Info");

        // ===== Public Endpoint =====
        churchInfo.MapGet("/", GetAsync)
                  .WithName("GetChurchInfo")
                  .WithSummary("Get church address, contact methods, and About-section content")
                  .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
                  .ProducesProblem(StatusCodes.Status404NotFound)
                  .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = churchInfo.MapGroup("/admin")
                              .RequireAuthorization("RequireContentEditor");

        admin.MapPut("/address", UpdateAddressAsync)
             .WithName("UpdateChurchAddress")
             .WithSummary("Update the church's address")
             .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapPut("/about", UpdateAboutSectionAsync)
             .WithName("UpdateChurchAboutSection")
             .WithSummary("Update the church's About-section content")
             .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> GetAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetChurchInfoQuery();
        var info = await mediator.Send(query, ct);
        return TypedResults.Ok(info);
    }

    private static async Task<IResult> UpdateAddressAsync(
        [FromBody] UpdateChurchAddressCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UpdateAboutSectionAsync(
        [FromBody] UpdateChurchAboutSectionCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    }
}