using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Commands;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Dtos;
using RccgHopeHouse.Application.Features.ThemesOfTheYear.Queries;

namespace RccgHopeHouse.Api.Endpoints.ThemesOfTheYear;

/// <summary>
/// Endpoints for retrieving and managing annual church themes.
/// 
/// Public endpoints provide the current theme and historical
/// themes by year.
/// 
/// Administrative endpoints allow administrators to view,
/// create and update Theme of the Year records.
/// </summary>
public static class ThemeOfTheYearEndpoints
{
    public static RouteGroupBuilder MapThemeOfTheYearEndpoints(
        this RouteGroupBuilder group)
    {
        var themes = group
            .MapGroup("/themes-of-the-year")
            .WithTags("Theme of the Year");

        // ==================== Public ====================

        themes.MapGet(
                "/current",
                GetCurrentAsync)
            .WithName("GetCurrentThemeOfTheYear")
            .WithSummary("Get the current Theme of the Year")
            .Produces<ThemeOfTheYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        themes.MapGet(
                "/{year:int}",
                GetByYearAsync)
            .WithName("GetThemeOfTheYearByYear")
            .WithSummary("Get the Theme of the Year for a specified year")
            .Produces<ThemeOfTheYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ==================== Admin ====================

        var admin = themes
            .MapGroup("/admin")
            .RequireAuthorization("RequireAdmin");

        admin.MapGet(
                "",
                GetAllAsync)
            .WithName("GetAllThemesOfTheYear")
            .WithSummary("Get all Theme of the Year records")
            .Produces<IReadOnlyList<ThemeOfTheYearDto>>(
                StatusCodes.Status200OK);

        admin.MapGet(
                "/{id:guid}",
                GetByIdAsync)
            .WithName("GetThemeOfTheYearById")
            .WithSummary("Get a Theme of the Year by ID")
            .Produces<ThemeOfTheYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "",
                CreateAsync)
            .WithName("CreateThemeOfTheYear")
            .WithSummary("Create a new Theme of the Year")
            .Produces<ThemeOfTheYearDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapPut(
                "/{id:guid}",
                UpdateAsync)
            .WithName("UpdateThemeOfTheYear")
            .WithSummary("Update an existing Theme of the Year")
            .Produces<ThemeOfTheYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        return group;
    }

    // ==================== Public Queries ====================

    private static async Task<IResult> GetCurrentAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetCurrentThemeOfTheYearQuery(),
            ct);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByYearAsync(
        int year,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetThemeOfTheYearByYearQuery(year),
            ct);

        if (result is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result);
    }

    // ==================== Admin Queries ====================

    private static async Task<IResult> GetAllAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var themes = await mediator.Send(
            new GetThemeOfTheYearListQuery(),
            ct);

        return TypedResults.Ok(themes);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var theme = await mediator.Send(
            new GetThemeOfTheYearByIdQuery(id),
            ct);

        return TypedResults.Ok(theme);
    }

    // ==================== Admin Commands ====================

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateThemeOfTheYearCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command,
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetThemeOfTheYearById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateThemeOfTheYearCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }
}