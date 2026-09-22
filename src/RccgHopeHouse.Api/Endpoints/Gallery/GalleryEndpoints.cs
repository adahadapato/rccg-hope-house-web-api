using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Gallery.Commands;
using RccgHopeHouse.Application.Features.Gallery.Dtos;
using RccgHopeHouse.Application.Features.Gallery.Queries;

namespace RccgHopeHouse.Api.Endpoints.Gallery;

public static class GalleryEndpoints
{
    public static RouteGroupBuilder MapGalleryEndpoints(
        this RouteGroupBuilder group)
    {
        var gallery = group
            .MapGroup("/gallery")
            .WithTags("Gallery");

        // ==================== Public ====================

        gallery.MapGet("/", GetFeedAsync)
            .WithName("GetGalleryFeed")
            .Produces<IReadOnlyList<GalleryImageFeedDto>>()
            .AllowAnonymous();

        gallery.MapGet("/tags", GetTagsAsync)
            .WithName("GetGalleryTags")
            .Produces<IReadOnlyList<GalleryTagDto>>()
            .AllowAnonymous();

        gallery.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetGalleryImageById")
            .Produces<GalleryImageDto>()
            .AllowAnonymous();

        gallery.MapGet("/{id:guid}/image", GetImageAsync)
            .WithName("GetGalleryImage")
            .Produces(StatusCodes.Status302Found)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        gallery.MapGet("/{id:guid}/thumbnail", GetThumbnailAsync)
            .WithName("GetGalleryThumbnail")
            .Produces(StatusCodes.Status302Found)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ==================== Admin ====================

        var admin = gallery
            .MapGroup("/admin")
            .RequireAuthorization(
                "RequireMediaManager");

        admin.MapGet("/", GetAdminImagesAsync)
            .WithName("GetAdminGalleryImages")
            .Produces<IReadOnlyList<GalleryImageDto>>();

        admin.MapPost("/upload", UploadAsync)
            .WithName("UploadGalleryImage")
            .Produces<GalleryImageDto>(
                StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .DisableAntiforgery()
            .ExcludeFromDescription();

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateGalleryImage")
            .Produces<GalleryImageDto>()
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .DisableAntiforgery()
            .ExcludeFromDescription();

        admin.MapPost(
                "/{id:guid}/featured",
                SetFeaturedAsync)
            .WithName("SetGalleryImageFeatured")
            .Produces<GalleryImageDto>();

        admin.MapPost(
                "/{id:guid}/visibility",
                ToggleVisibilityAsync)
            .WithName("ToggleGalleryImageVisibility")
            .Produces<GalleryImageDto>();

        admin.MapDelete(
                "/{id:guid}",
                DeleteAsync)
            .WithName("DeleteGalleryImage")
            .Produces(
                StatusCodes.Status204NoContent);

        return group;
    }

    // ==================== Public handlers ====================

    private static async Task<IResult> GetFeedAsync(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? tagId,
        [FromQuery] bool? isFeatured,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var query =
            new GetGalleryFeedQuery(
                categoryId,
                tagId,
                isFeatured,
                skip,
                take);

        var images =
            await mediator.Send(
                query,
                ct);

        return TypedResults.Ok(
            images);
    }

    private static async Task<IResult> GetTagsAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var tags =
            await mediator.Send(
                new GetGalleryTagsQuery(),
                ct);

        return TypedResults.Ok(
            tags);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var image =
            await mediator.Send(
                new GetGalleryImageByIdQuery(
                    id),
                ct);

        return TypedResults.Ok(
            image);
    }

    private static async Task<IResult> GetImageAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var image =
            await mediator.Send(
                new GetGalleryImageByIdQuery(
                    id),
                ct);

        if (string.IsNullOrWhiteSpace(
                image.ImagePath))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Redirect(
            image.ImagePath,
            permanent: false,
            preserveMethod: false);
    }

    private static async Task<IResult> GetThumbnailAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var image =
            await mediator.Send(
                new GetGalleryImageByIdQuery(
                    id),
                ct);

        var path =
            !string.IsNullOrWhiteSpace(
                image.ThumbnailPath)
                ? image.ThumbnailPath
                : image.ImagePath;

        if (string.IsNullOrWhiteSpace(
                path))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Redirect(
            path,
            permanent: false,
            preserveMethod: false);
    }

    // ==================== Admin handlers ====================

    private static async Task<IResult> GetAdminImagesAsync(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? tagId,
        [FromQuery] bool? isFeatured,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var query =
            new GetAdminGalleryImagesQuery(
                categoryId,
                tagId,
                isFeatured,
                skip,
                take <= 0
                    ? 100
                    : take);

        var images =
            await mediator.Send(
                query,
                ct);

        return TypedResults.Ok(
            images);
    }

    private static async Task<IResult> UploadAsync(
        [AsParameters]
        UploadGalleryImageRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        if (request.File is null ||
            request.File.Length == 0)
        {
            return TypedResults.BadRequest(
                "Image file is required.");
        }

        using var ms =
            new MemoryStream();

        await request.File.CopyToAsync(
            ms,
            ct);

        var tags =
            ParseTags(
                request.Tags);

        var command =
            new UploadGalleryImageCommand(
                ImageData:
                    ms.ToArray(),
                ContentType:
                    request.File.ContentType,
                CategoryId:
                    request.CategoryId,
                Title:
                    request.Title,
                AltText:
                    request.AltText,
                Description:
                    request.Description,
                EventDate:
                    request.EventDate,
                Photographer:
                    request.Photographer,
                Tags:
                    tags,
                DisplayOrder:
                    request.DisplayOrder);

        var result =
            await mediator.Send(
                command,
                ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetGalleryImageById",
            new
            {
                id = result.Id
            });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [AsParameters]
        UpdateGalleryImageRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        byte[]? newImageData = null;
        string? newContentType = null;

        if (request.NewImage is not null &&
            request.NewImage.Length > 0)
        {
            using var ms =
                new MemoryStream();

            await request.NewImage.CopyToAsync(
                ms,
                ct);

            newImageData =
                ms.ToArray();

            newContentType =
                request.NewImage.ContentType;
        }

        var tags =
            ParseTags(
                request.Tags);

        var command =
            new UpdateGalleryImageCommand(
                Id:
                    id,
                Title:
                    request.Title,
                Description:
                    request.Description,
                AltText:
                    request.AltText,
                Photographer:
                    request.Photographer,
                NewImageData:
                    newImageData,
                NewContentType:
                    newContentType,
                EventDate:
                    request.EventDate,
                CategoryId:
                    request.CategoryId,
                Tags:
                    tags,
                DisplayOrder:
                    request.DisplayOrder);

        var result =
            await mediator.Send(
                command,
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> SetFeaturedAsync(
        Guid id,
        [FromBody]
        SetFeaturedRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new SetImageFeaturedCommand(
                    id,
                    request.IsFeatured),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> ToggleVisibilityAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var result =
            await mediator.Send(
                new ToggleImageVisibilityCommand(
                    id),
                ct);

        return TypedResults.Ok(
            result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(
            new DeleteGalleryImageCommand(
                id),
            ct);

        return TypedResults.NoContent();
    }

    private static List<string>? ParseTags(
        string? rawTags)
    {
        if (string.IsNullOrWhiteSpace(
                rawTags))
        {
            return new List<string>();
        }

        return rawTags
            .Split(
                ',',
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries)
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

public record SetFeaturedRequest(
    bool IsFeatured);

public class UploadGalleryImageRequest
{
    [FromForm]
    public IFormFile File { get; set; } =
        null!;

    [FromForm]
    public string Title { get; set; } =
        string.Empty;

    [FromForm]
    public string AltText { get; set; } =
        string.Empty;

    [FromForm]
    public Guid CategoryId { get; set; }

    [FromForm]
    public string? Description { get; set; }

    [FromForm]
    public DateTime? EventDate { get; set; }

    [FromForm]
    public string? Photographer { get; set; }

    [FromForm]
    public string? Tags { get; set; }

    [FromForm]
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateGalleryImageRequest
{
    [FromForm]
    public string Title { get; set; } =
        string.Empty;

    [FromForm]
    public string? Description { get; set; }

    [FromForm]
    public string AltText { get; set; } =
        string.Empty;

    [FromForm]
    public string? Photographer { get; set; }

    [FromForm]
    public IFormFile? NewImage { get; set; }

    [FromForm]
    public DateTime? EventDate { get; set; }

    [FromForm]
    public Guid CategoryId { get; set; }

    [FromForm]
    public string? Tags { get; set; }

    [FromForm]
    public int DisplayOrder { get; set; } = 0;
}