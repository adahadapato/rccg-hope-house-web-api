using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RccgHopeHouse.Application.Features.Gallery.Commands;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Application.Features.Gallery.Queries;

namespace RccgHopeHouse.Api.Endpoints.Gallery;

/// <summary>
/// Minimal API endpoints for the church gallery: public image feed, categories, tags, and admin upload/management.
/// Supports byte[] image storage with streaming responses for efficient delivery.
/// </summary>
public static class GalleryEndpoints
{
    /// <summary>
    /// Registers all gallery endpoints under the /gallery route group.
    /// Public endpoints allow anonymous access; admin endpoints require MediaManager role.
    /// </summary>
    public static RouteGroupBuilder MapGalleryEndpoints(this RouteGroupBuilder group)
    {
        var gallery = group.MapGroup("/gallery")
                           .WithTags("Gallery")
                           .WithOpenApi();

        // ===== Public Endpoints =====
        gallery.MapGet("/", GetFeedAsync)
               .WithName("GetGalleryFeed")
               .WithOpenApi(x => new(x)
               {
                   Summary = "Get public gallery feed",
                   Description = "Returns paginated, filtered list of gallery images with thumbnails. Excludes full binary data."
               })
               .Produces<IReadOnlyList<GalleryImageFeedDto>>()
               .AllowAnonymous();

        gallery.MapGet("/categories", GetCategoriesAsync)
               .WithName("GetGalleryCategories")
               .WithOpenApi(x => new(x) { Summary = "Get gallery categories for filtering" })
               .Produces<IReadOnlyList<GalleryCategoryDto>>()
               .AllowAnonymous();

        gallery.MapGet("/tags", GetTagsAsync)
               .WithName("GetGalleryTags")
               .WithOpenApi(x => new(x) { Summary = "Get gallery tags for filtering" })
               .Produces<IReadOnlyList<GalleryTagDto>>()
               .AllowAnonymous();

        gallery.MapGet("/{id:guid}", GetByIdAsync)
               .WithName("GetGalleryImageById")
               .WithOpenApi(x => new(x) { Summary = "Get single image by ID" })
               .Produces<GalleryImageDto>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        // Stream image binary data directly from database
        gallery.MapGet("/{id:guid}/image", StreamImageAsync)
               .WithName("StreamGalleryImage")
               .WithOpenApi(x => new(x) { Summary = "Stream image binary data" })
               .Produces<byte[]>(StatusCodes.Status200OK, "image/jpeg")
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        gallery.MapGet("/{id:guid}/thumbnail", StreamThumbnailAsync)
               .WithName("StreamGalleryThumbnail")
               .WithOpenApi(x => new(x) { Summary = "Stream thumbnail binary data" })
               .Produces<byte[]>(StatusCodes.Status200OK, "image/jpeg")
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = gallery.MapGroup("/admin")
                           .RequireAuthorization("RequireMediaManager");

        admin.MapPost("/upload", UploadAsync)
             .WithName("UploadGalleryImage")
             .WithOpenApi(x => new(x)
             {
                 Summary = "Upload new gallery image",
                 Description = "Accepts multipart/form-data with image file and metadata. Optimizes image before DB storage."
             })
             .Produces<GalleryImageDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem()
             .DisableAntiforgery(); // For API clients; use CSRF protection for browser forms

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateGalleryImage")
             .WithOpenApi(x => new(x) { Summary = "Update image metadata or replace binary data" })
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem();

        admin.MapPost("/{id:guid}/featured", SetFeaturedAsync)
             .WithName("SetGalleryImageFeatured")
             .WithOpenApi(x => new(x) { Summary = "Toggle featured status for homepage highlights" })
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/visibility", ToggleVisibilityAsync)
             .WithName("ToggleGalleryImageVisibility")
             .WithOpenApi(x => new(x) { Summary = "Toggle public/private visibility" })
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
             .WithName("DeleteGalleryImage")
             .WithOpenApi(x => new(x) { Summary = "Permanently delete image from database" })
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public Handlers =====
    private static async Task<IResult> GetFeedAsync(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? tagId,
        [FromQuery] bool? isFeatured,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryFeedQuery(categoryId, tagId, isFeatured, skip, take);
        var images = await mediator.Send(query, ct);
        return TypedResults.Ok(images);
    }

    private static async Task<IResult> GetCategoriesAsync(
        [FromQuery] bool includeInactive,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryCategoriesQuery(includeInactive);
        var categories = await mediator.Send(query, ct);
        return TypedResults.Ok(categories);
    }

    private static async Task<IResult> GetTagsAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetGalleryTagsQuery();
        var tags = await mediator.Send(query, ct);
        return TypedResults.Ok(tags);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryImageByIdQuery(id);
        var image = await mediator.Send(query, ct);
        return TypedResults.Ok(image);
    }

    /// <summary>
    /// Streams full-resolution image binary data with proper Content-Type header.
    /// Uses AsNoTracking projection in repository to avoid loading unnecessary fields.
    /// </summary>
    private static async Task<IResult> StreamImageAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryImageByIdQuery(id);
        var image = await mediator.Send(query, ct);

        if (image?.ImageData == null)
            return TypedResults.NotFound();

        return TypedResults.File(image.ImageData, image.ContentType, fileDownloadName: $"{image.Title}.jpg");
    }

    /// <summary>
    /// Streams thumbnail binary data for fast grid rendering.
    /// Falls back to resizing full image if thumbnail not available.
    /// </summary>
    private static async Task<IResult> StreamThumbnailAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryImageByIdQuery(id);
        var image = await mediator.Send(query, ct);

        if (image?.ImageData == null)
            return TypedResults.NotFound();

        var thumbnail = image.ThumbnailData ?? image.ImageData; // Fallback to full image
        return TypedResults.File(thumbnail, "image/jpeg", fileDownloadName: $"{image.Title}_thumb.jpg");
    }

    // ===== Admin Handlers =====
    private static async Task<IResult> UploadAsync(
        [FromForm] IFormFile file,
        [FromForm] string title,
        [FromForm] string altText,
        [FromForm] Guid categoryId,
        [FromForm] string? description,
        [FromForm] DateTime? eventDate,
        [FromForm] string? photographer,
        [FromForm] List<string>? tags,
        IMediator mediator,
        CancellationToken ct)
    {
        if (file.Length == 0)
            return TypedResults.BadRequest("Image file is required.");

        // Read file into byte[] for Application layer processing
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var imageData = ms.ToArray();

        var command = new UploadGalleryImageCommand(
            imageData,
            file.ContentType,
            categoryId,
            title,
            altText,
            description,
            eventDate,
            photographer,
            tags);

        var result = await mediator.Send(command, ct);
        return TypedResults.CreatedAtRoute(result, "GetGalleryImageById", new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromForm] string title,
        [FromForm] string? description,
        [FromForm] string altText,
        [FromForm] string? photographer,
        [FromForm] IFormFile? newImage,
        [FromForm] DateTime? eventDate,
        IMediator mediator,
        CancellationToken ct)
    {
        byte[]? newImageData = null;
        string? newContentType = null;

        if (newImage?.Length > 0)
        {
            using var ms = new MemoryStream();
            await newImage.CopyToAsync(ms, ct);
            newImageData = ms.ToArray();
            newContentType = newImage.ContentType;
        }

        var command = new UpdateGalleryImageCommand(
            id, title, description, altText, photographer,
            newImageData, newContentType, eventDate);

        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> SetFeaturedAsync(
        Guid id,
        [FromBody] SetFeaturedRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new SetImageFeaturedCommand(id, request.IsFeatured);
        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> ToggleVisibilityAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new ToggleImageVisibilityCommand(id);
        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteGalleryImageCommand(id);
        await mediator.Send(command, ct);
        return TypedResults.NoContent();
    }
}

// ==================== API Layer Request DTOs ====================
public record SetFeaturedRequest(bool IsFeatured);