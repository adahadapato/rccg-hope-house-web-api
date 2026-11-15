using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Prophecies.Commands;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Application.Features.Prophecies.Queries;
using RccgHopeHouse.Application.Features.ProphecyCategories.Commands;
using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Application.Features.ProphecyCategories.Queries;
using RccgHopeHouse.Application.Features.ProphecyYears.Commands;
using RccgHopeHouse.Application.Features.ProphecyYears.Dtos;
using RccgHopeHouse.Application.Features.ProphecyYears.Queries;

namespace RccgHopeHouse.Api.Endpoints.Prophecies;

/// <summary>
/// Endpoints for retrieving and managing RCCG prophecies.
///
/// Published prophecy years and their active categories and
/// prophecy statements are publicly accessible.
///
/// Administrative operations for years, categories and individual
/// prophecy statements require administrator authorization.
/// </summary>
public static class ProphecyEndpoints
{
    public static RouteGroupBuilder MapProphecyEndpoints(
        this RouteGroupBuilder group)
    {
        var prophecies = group
            .MapGroup("/prophecies")
            .WithTags("Prophecies");

        // ==================== Public ====================

        prophecies.MapGet(
                "/years",
                GetPublishedYearsAsync)
            .WithName("GetPublishedProphecyYears")
            .WithSummary("Get all published prophecy years")
            .Produces<IReadOnlyList<ProphecyYearDto>>(
                StatusCodes.Status200OK)
            .AllowAnonymous();

        prophecies.MapGet(
                "/latest",
                GetLatestAsync)
            .WithName("GetLatestProphecies")
            .WithSummary("Get the latest published prophecies")
            .Produces<ProphecyYearDetailDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        prophecies.MapGet(
                "/{year:int}",
                GetByYearAsync)
            .WithName("GetPropheciesByYear")
            .WithSummary("Get published prophecies for a specified year")
            .Produces<ProphecyYearDetailDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ==================== Admin ====================

        var admin = prophecies
            .MapGroup("/admin")
            .RequireAuthorization("RequireAdmin");

        // ==================== Admin Years ====================

        admin.MapGet(
                "/years",
                GetAllYearsAsync)
            .WithName("GetAllProphecyYears")
            .WithSummary("Get all prophecy years including unpublished ones")
            .Produces<IReadOnlyList<ProphecyYearDto>>(
                StatusCodes.Status200OK);

        admin.MapGet(
                "/years/{id:guid}",
                GetYearByIdAsync)
            .WithName("GetProphecyYearById")
            .WithSummary("Get a prophecy year by ID")
            .Produces<ProphecyYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/years",
                CreateYearAsync)
            .WithName("CreateProphecyYear")
            .WithSummary("Create a new prophecy year")
            .Produces<ProphecyYearDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapPut(
                "/years/{id:guid}",
                UpdateYearAsync)
            .WithName("UpdateProphecyYear")
            .WithSummary("Update an existing prophecy year")
            .Produces<ProphecyYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPost(
                "/years/{id:guid}/publish",
                PublishYearAsync)
            .WithName("PublishProphecyYear")
            .WithSummary("Publish a prophecy year")
            .Produces<ProphecyYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/years/{id:guid}/unpublish",
                UnpublishYearAsync)
            .WithName("UnpublishProphecyYear")
            .WithSummary("Unpublish a prophecy year")
            .Produces<ProphecyYearDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        // ==================== Admin Categories ====================

        admin.MapGet(
                "/years/{prophecyYearId:guid}/categories",
                GetCategoriesByYearAsync)
            .WithName("GetProphecyCategoriesByYear")
            .WithSummary("Get categories belonging to a prophecy year")
            .Produces<IReadOnlyList<ProphecyCategoryDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapGet(
                "/categories/{id:guid}",
                GetCategoryByIdAsync)
            .WithName("GetProphecyCategoryById")
            .WithSummary("Get a prophecy category by ID")
            .Produces<ProphecyCategoryDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/categories",
                CreateCategoryAsync)
            .WithName("CreateProphecyCategory")
            .WithSummary("Create a new prophecy category")
            .Produces<ProphecyCategoryDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapPut(
                "/categories/{id:guid}",
                UpdateCategoryAsync)
            .WithName("UpdateProphecyCategory")
            .WithSummary("Update an existing prophecy category")
            .Produces<ProphecyCategoryDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPost(
                "/categories/{id:guid}/activate",
                ActivateCategoryAsync)
            .WithName("ActivateProphecyCategory")
            .WithSummary("Activate a prophecy category")
            .Produces<ProphecyCategoryDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/categories/{id:guid}/deactivate",
                DeactivateCategoryAsync)
            .WithName("DeactivateProphecyCategory")
            .WithSummary("Deactivate a prophecy category")
            .Produces<ProphecyCategoryDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        // ==================== Admin Prophecies ====================

        admin.MapGet(
                "/categories/{categoryId:guid}/prophecies",
                GetPropheciesByCategoryAsync)
            .WithName("GetPropheciesByCategory")
            .WithSummary("Get prophecies belonging to a category")
            .Produces<IReadOnlyList<ProphecyDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapGet(
                "/items/{id:guid}",
                GetProphecyByIdAsync)
            .WithName("GetProphecyById")
            .WithSummary("Get an individual prophecy by ID")
            .Produces<ProphecyDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/items",
                CreateProphecyAsync)
            .WithName("CreateProphecy")
            .WithSummary("Create an individual prophecy")
            .Produces<ProphecyDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapPut(
                "/items/{id:guid}",
                UpdateProphecyAsync)
            .WithName("UpdateProphecy")
            .WithSummary("Update an individual prophecy")
            .Produces<ProphecyDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPost(
                "/items/{id:guid}/activate",
                ActivateProphecyAsync)
            .WithName("ActivateProphecy")
            .WithSummary("Activate an individual prophecy")
            .Produces<ProphecyDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        admin.MapPost(
                "/items/{id:guid}/deactivate",
                DeactivateProphecyAsync)
            .WithName("DeactivateProphecy")
            .WithSummary("Deactivate an individual prophecy")
            .Produces<ProphecyDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound);

        return group;
    }

    // ==================== Public Queries ====================

    private static async Task<IResult> GetPublishedYearsAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var years = await mediator.Send(
            new GetPublishedProphecyYearsQuery(),
            ct);

        return TypedResults.Ok(years);
    }

    private static async Task<IResult> GetLatestAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetLatestPropheciesQuery(),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByYearAsync(
        int year,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetPropheciesByYearQuery(year),
            ct);

        return TypedResults.Ok(result);
    }

    // ==================== Admin Year Queries ====================

    private static async Task<IResult> GetAllYearsAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var years = await mediator.Send(
            new GetAllProphecyYearsQuery(),
            ct);

        return TypedResults.Ok(years);
    }

    private static async Task<IResult> GetYearByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var year = await mediator.Send(
            new GetProphecyYearByIdQuery(id),
            ct);

        return TypedResults.Ok(year);
    }

    // ==================== Admin Year Commands ====================

    private static async Task<IResult> CreateYearAsync(
        [FromBody] CreateProphecyYearCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command,
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetProphecyYearById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateYearAsync(
        Guid id,
        [FromBody] UpdateProphecyYearCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> PublishYearAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new PublishProphecyYearCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UnpublishYearAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new UnpublishProphecyYearCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    // ==================== Admin Category Queries ====================

    private static async Task<IResult> GetCategoriesByYearAsync(
        Guid prophecyYearId,
        [FromQuery] bool activeOnly,
        IMediator mediator,
        CancellationToken ct)
    {
        var categories = await mediator.Send(
            new GetProphecyCategoriesByYearQuery(
                prophecyYearId,
                activeOnly),
            ct);

        return TypedResults.Ok(categories);
    }

    private static async Task<IResult> GetCategoryByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var category = await mediator.Send(
            new GetProphecyCategoryByIdQuery(id),
            ct);

        return TypedResults.Ok(category);
    }

    // ==================== Admin Category Commands ====================

    private static async Task<IResult> CreateCategoryAsync(
        [FromBody] CreateProphecyCategoryCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command,
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetProphecyCategoryById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateCategoryAsync(
        Guid id,
        [FromBody] UpdateProphecyCategoryCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ActivateCategoryAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ActivateProphecyCategoryCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeactivateCategoryAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeactivateProphecyCategoryCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    // ==================== Admin Prophecy Queries ====================

    private static async Task<IResult> GetPropheciesByCategoryAsync(
        Guid categoryId,
        [FromQuery] bool activeOnly,
        IMediator mediator,
        CancellationToken ct)
    {
        var prophecies = await mediator.Send(
            new GetPropheciesByCategoryQuery(
                categoryId,
                activeOnly),
            ct);

        return TypedResults.Ok(prophecies);
    }

    private static async Task<IResult> GetProphecyByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var prophecy = await mediator.Send(
            new GetProphecyByIdQuery(id),
            ct);

        return TypedResults.Ok(prophecy);
    }

    // ==================== Admin Prophecy Commands ====================

    private static async Task<IResult> CreateProphecyAsync(
        [FromBody] CreateProphecyCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command,
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetProphecyById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateProphecyAsync(
        Guid id,
        [FromBody] UpdateProphecyCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ActivateProphecyAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ActivateProphecyCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeactivateProphecyAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeactivateProphecyCommand(id),
            ct);

        return TypedResults.Ok(result);
    }
}