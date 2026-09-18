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
    public static RouteGroupBuilder MapGalleryEndpoints(this RouteGroupBuilder group)
    {
        var gallery = group.MapGroup("/gallery")
                           .WithTags("Gallery");

        // ===== Public Endpoints =====
        gallery.MapGet("/", GetFeedAsync)
               .WithName("GetGalleryFeed")
               .WithSummary("Get public gallery feed")
               .WithDescription("Returns paginated, filtered list of gallery images with thumbnails. Excludes full binary data.")
               .Produces<IReadOnlyList<GalleryImageFeedDto>>()
               .AllowAnonymous();

        gallery.MapGet("/categories", GetCategoriesAsync)
               .WithName("GetGalleryCategories")
               .WithSummary("Get gallery categories for filtering")
               .Produces<IReadOnlyList<GalleryCategoryDto>>()
               .AllowAnonymous();

        gallery.MapGet("/tags", GetTagsAsync)
               .WithName("GetGalleryTags")
               .WithSummary("Get gallery tags for filtering")
               .Produces<IReadOnlyList<GalleryTagDto>>()
               .AllowAnonymous();

        gallery.MapGet("/{id:guid}", GetByIdAsync)
               .WithName("GetGalleryImageById")
               .WithSummary("Get single image by ID")
               .Produces<GalleryImageDto>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        gallery.MapGet("/{id:guid}/image", StreamImageAsync)
               .WithName("StreamGalleryImage")
               .WithSummary("Stream image binary data")
               .Produces<byte[]>(StatusCodes.Status200OK, "image/jpeg")
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        gallery.MapGet("/{id:guid}/thumbnail", StreamThumbnailAsync)
               .WithName("StreamGalleryThumbnail")
               .WithSummary("Stream thumbnail binary data")
               .Produces<byte[]>(StatusCodes.Status200OK, "image/jpeg")
               .ProducesProblem(StatusCodes.Status404NotFound)
               .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = gallery.MapGroup("/admin")
                           .RequireAuthorization("RequireMediaManager");

        admin.MapPost("/upload", UploadAsync)
             .WithName("UploadGalleryImage")
             .WithSummary("Upload new gallery image")
             .WithDescription("Accepts multipart/form-data with image file and metadata. Optimizes image before DB storage.")
             .Produces<GalleryImageDto>(StatusCodes.Status201Created)
             .ProducesValidationProblem()
             .DisableAntiforgery()
             .ExcludeFromDescription();

        admin.MapPut("/{id:guid}", UpdateAsync)
             .WithName("UpdateGalleryImage")
             .WithSummary("Update image metadata or replace binary data")
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem()
             .DisableAntiforgery()
             .ExcludeFromDescription();

        admin.MapPost("/{id:guid}/featured", SetFeaturedAsync)
             .WithName("SetGalleryImageFeatured")
             .WithSummary("Toggle featured status for homepage highlights")
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/visibility", ToggleVisibilityAsync)
             .WithName("ToggleGalleryImageVisibility")
             .WithSummary("Toggle public/private visibility")
             .Produces<GalleryImageDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
             .WithName("DeleteGalleryImage")
             .WithSummary("Permanently delete image from database")
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

    private static async Task<IResult> StreamThumbnailAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetGalleryImageByIdQuery(id);
        var image = await mediator.Send(query, ct);

        if (image?.ImageData == null)
            return TypedResults.NotFound();

        var thumbnail = image.ThumbnailData ?? image.ImageData;
        return TypedResults.File(thumbnail, "image/jpeg", fileDownloadName: $"{image.Title}_thumb.jpg");
    }

    // ===== Admin Handlers =====

    /// <summary>
    /// Uses [AsParameters] with a bound request class rather than individual
    /// [FromForm] parameters — Swashbuckle cannot generate an OpenAPI schema
    /// for a Minimal API action mixing [FromForm] IFormFile with multiple
    /// other [FromForm] scalars directly; binding them into one class
    /// resolves this cleanly.
    /// </summary>
    private static async Task<IResult> UploadAsync(
    [AsParameters] UploadGalleryImageRequest request,
    IMediator mediator,
    CancellationToken ct)
    {
        if (request.File.Length == 0)
            return TypedResults.BadRequest("Image file is required.");

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms, ct);
        var imageData = ms.ToArray();

        var tags = string.IsNullOrWhiteSpace(request.Tags)
            ? null
            : request.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

        var command = new UploadGalleryImageCommand(
            imageData,
            request.File.ContentType,
            request.CategoryId,
            request.Title,
            request.AltText,
            request.Description,
            request.EventDate,
            request.Photographer,
            tags);

        var result = await mediator.Send(command, ct);
        return TypedResults.CreatedAtRoute(result, "GetGalleryImageById", new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [AsParameters] UpdateGalleryImageRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        byte[]? newImageData = null;
        string? newContentType = null;

        if (request.NewImage?.Length > 0)
        {
            using var ms = new MemoryStream();
            await request.NewImage.CopyToAsync(ms, ct);
            newImageData = ms.ToArray();
            newContentType = request.NewImage.ContentType;
        }

        var command = new UpdateGalleryImageCommand(
            id, request.Title, request.Description, request.AltText, request.Photographer,
            newImageData, newContentType, request.EventDate);

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

public class UploadGalleryImageRequest
{
    [FromForm] public IFormFile File { get; set; } = null!;
    [FromForm] public string Title { get; set; } = string.Empty;
    [FromForm] public string AltText { get; set; } = string.Empty;
    [FromForm] public Guid CategoryId { get; set; }
    [FromForm] public string? Description { get; set; }
    [FromForm] public DateTime? EventDate { get; set; }
    [FromForm] public string? Photographer { get; set; }

    /// <summary>
    /// Comma-separated tag names, e.g. "easter,youth,2026". Split into a
    /// list in the handler. Using a single delimited string rather than
    /// List&lt;string&gt; sidesteps a known ASP.NET Core Minimal API bug
    /// where the form-binding expression compiler fails to build a request
    /// delegate for List&lt;string&gt; properties on [AsParameters] types.
    /// </summary>
    [FromForm] public string? Tags { get; set; }
}

public class UpdateGalleryImageRequest
{
    [FromForm] public string Title { get; set; } = string.Empty;
    [FromForm] public string? Description { get; set; }
    [FromForm] public string AltText { get; set; } = string.Empty;
    [FromForm] public string? Photographer { get; set; }
    [FromForm] public IFormFile? NewImage { get; set; }
    [FromForm] public DateTime? EventDate { get; set; }
}