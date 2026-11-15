using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.GalleryCategories.Commands;
using RccgHopeHouse.Application.Features.GalleryCategories.Dtos;
using RccgHopeHouse.Application.Features.GalleryCategories.Queries;

namespace RccgHopeHouse.Api.Endpoints.GalleryCategories;

/// <summary>
/// Endpoints for managing and retrieving gallery categories.
///
/// Active gallery categories are publicly accessible so that the
/// gallery and image-upload interfaces can populate category selections.
///
/// Administrative operations such as creating, updating, activating and
/// deactivating gallery categories require administrator authorization.
/// </summary>
public static class GalleryCategoryEndpoints
{
    public static RouteGroupBuilder MapGalleryCategoryEndpoints(
        this RouteGroupBuilder group)
    {
        var categories = group.MapGroup("/gallery-categories")
                              .WithTags("Gallery Categories");

        // ==================== Public ====================

        categories.MapGet("/active", GetActiveAsync)
                  .WithName("GetActiveGalleryCategories")
                  .WithSummary("Get active gallery categories")
                  .Produces<IReadOnlyList<GalleryCategoryDto>>(
                      StatusCodes.Status200OK);

        // ==================== Admin ====================

        var admin = categories.MapGroup("/admin")
                              .RequireAuthorization("RequireAdmin");

        admin.MapGet("/", GetAllAsync)
             .WithName("GetGalleryCategories")
             .WithSummary("Get all gallery categories including inactive ones")
             .Produces<IReadOnlyList<GalleryCategoryDto>>(
                 StatusCodes.Status200OK);

        admin.MapGet("/{id:guid}", GetByIdAsync)
             .WithName("GetGalleryCategoryById")
             .WithSummary("Get a gallery category by ID")
             .Produces<GalleryCategoryDto>(
                 StatusCodes.Status200OK)
             .ProducesProblem(
                 StatusCodes.Status404NotFound);

        admin.MapPost("/", CreateAsync)
             .WithName("CreateGalleryCategory")
             .WithSummary("Create a new gallery category")
             .Produces<GalleryCategoryDto>(
                 StatusCodes.Status201Created)
             .ProducesValidationProblem();

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateGalleryCategory")
             .WithSummary("Update an existing gallery category")
             .Produces<GalleryCategoryDto>(
                 StatusCodes.Status200OK)
             .ProducesProblem(
                 StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapPost("/{id:guid}/deactivate", DeactivateAsync)
             .WithName("DeactivateGalleryCategory")
             .WithSummary("Deactivate a gallery category")
             .Produces<GalleryCategoryDto>(
                 StatusCodes.Status200OK)
             .ProducesProblem(
                 StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/activate", ActivateAsync)
             .WithName("ActivateGalleryCategory")
             .WithSummary("Reactivate a previously deactivated gallery category")
             .Produces<GalleryCategoryDto>(
                 StatusCodes.Status200OK)
             .ProducesProblem(
                 StatusCodes.Status404NotFound);

        return group;
    }

    // ==================== Public Queries ====================

    /// <summary>
    /// Gets active gallery categories for public gallery use.
    /// </summary>
    private static async Task<IResult> GetActiveAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetActiveGalleryCategoriesQuery();

        var list = await mediator.Send(query, ct);

        return TypedResults.Ok(list);
    }

    // ==================== Admin Queries ====================

    /// <summary>
    /// Gets all gallery categories, including inactive categories.
    /// </summary>
    private static async Task<IResult> GetAllAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetAllGalleryCategoriesQuery();

        var list = await mediator.Send(query, ct);

        return TypedResults.Ok(list);
    }

    /// <summary>
    /// Gets a single gallery category by its unique identifier.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryCategoryByIdQuery(id);

        var category = await mediator.Send(query, ct);

        return TypedResults.Ok(category);
    }

    // ==================== Admin Commands ====================

    /// <summary>
    /// Creates a new gallery category.
    /// </summary>
    private static async Task<IResult> CreateAsync(
        [FromBody] CreateGalleryCategoryCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetGalleryCategoryById",
            new { id = result.Id });
    }

    /// <summary>
    /// Updates an existing gallery category.
    ///
    /// The route ID is treated as the source of truth and replaces
    /// any ID supplied in the request body.
    /// </summary>
    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateGalleryCategoryCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            command with { Id = id },
            ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Deactivates a gallery category without deleting it.
    /// Existing images remain associated with the category.
    /// </summary>
    private static async Task<IResult> DeactivateAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new DeactivateGalleryCategoryCommand(id),
            ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Reactivates a previously deactivated gallery category.
    /// </summary>
    private static async Task<IResult> ActivateAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new ActivateGalleryCategoryCommand(id),
            ct);

        return TypedResults.Ok(result);
    }
}