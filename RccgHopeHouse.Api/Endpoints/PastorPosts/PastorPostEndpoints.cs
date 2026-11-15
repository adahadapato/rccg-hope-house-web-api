using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.PastorPosts.Commands;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Application.Features.PastorPosts.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.PastorPosts;

/// <summary>
/// Minimal API endpoints for Pastor's Corner:
/// public scrolling feed and admin CRUD.
/// </summary>
public static class PastorPostEndpoints
{
    public static RouteGroupBuilder MapPastorPostEndpoints(
        this RouteGroupBuilder group)
    {
        var posts = group
            .MapGroup("/pastor-posts")
            .WithTags("Pastor's Corner");

        // ============================================================
        // Public Endpoints
        // ============================================================

        posts.MapGet("/{id:guid}/siblings", GetSiblingsAsync)
            .WithSummary(
                "Get other topics under the same theme as this post")
            .WithDescription(
                "Returns other published posts sharing this post's ThemeOfTheYear, " +
                "for the 'other topics' selector on the article detail view.")
            .WithName("GetPastorPostSiblings")
            .Produces<IReadOnlyList<PastorPostFeedDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        posts.MapGet("/", GetFeedAsync)
            .WithSummary("Get public scrolling feed")
            .WithDescription(
                "Returns paginated, pinned-first feed of pastor posts. " +
                "Supports category filtering.")
            .WithName("GetPastorPostFeed")
            .Produces<IReadOnlyList<PastorPostFeedDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        posts.MapGet("/{id:guid}", GetByIdAsync)
            .WithSummary("Get single post by ID")
            .WithDescription(
                "Returns full post content, cover image, and metadata " +
                "for public detail view.")
            .WithName("GetPastorPostById")
            .Produces<PastorPostDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ============================================================
        // Admin Endpoints
        // ============================================================

        var admin = posts
            .MapGroup("/admin")
            .WithTags("Pastor's Corner - Admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapGet("/", GetAllForAdminAsync)
            .WithSummary("Get all pastor posts for admin")
            .WithDescription(
                "Returns pastor posts for the admin management panel. " +
                "Drafts are included by default.")
            .WithName("GetPastorPostsForAdmin")
            .Produces<IReadOnlyList<PastorPostDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/", CreateAsync)
            .WithSummary("Create new pastor post (draft)")
            .WithDescription(
                "Creates a post in draft status. " +
                "Use the publish endpoint to make it public.")
            .WithName("CreatePastorPost")
            .Produces<PastorPostDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithSummary("Update existing post")
            .WithDescription(
                "Updates title, content, category, theme, cover image, " +
                "or other metadata of an existing post.")
            .WithName("UpdatePastorPost")
            .Produces<PastorPostDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/publish", PublishAsync)
            .WithSummary("Publish a draft post")
            .WithDescription(
                "Changes post status from draft to published, " +
                "making it visible in the public feed.")
            .WithName("PublishPastorPost")
            .Produces<PastorPostDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/unpublish", UnpublishAsync)
            .WithSummary("Unpublish a published post")
            .WithDescription(
                "Removes the post from the public feed while keeping it " +
                "available for editing and later republishing.")
            .WithName("UnpublishPastorPost")
            .Produces<PastorPostDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/pin", PinAsync)
            .WithSummary("Pin or unpin post in feed")
            .WithDescription(
                "Sets the pinned status. Pinned posts appear at the top " +
                "of the public scrolling feed.")
            .WithName("PinPastorPost")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithSummary("Permanently delete post")
            .WithDescription(
                "Removes the post and its cover image from the database. " +
                "This action cannot be undone.")
            .WithName("DeletePastorPost")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    // ============================================================
    // Public Handlers
    // ============================================================

    private static async Task<IResult> GetFeedAsync(
        [FromQuery] PostCategory? category,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPastorPostFeedQuery(
            category,
            skip,
            take);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPastorPostByIdQuery(id);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetSiblingsAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPastorPostSiblingsQuery(id);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    // ============================================================
    // Admin Handlers
    // ============================================================

    private static async Task<IResult> GetAllForAdminAsync(
        [FromQuery] bool includeDrafts,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPastorPostsForAdminQuery(
            IncludeDrafts: includeDrafts);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreatePastorPostRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreatePastorPostCommand(
            Title: request.Title,
            Content: request.Content,
            Category: request.Category,
            ThemeOfTheYearId: request.ThemeOfTheYearId,
            Excerpt: request.Excerpt,
            IntroHeading: request.IntroHeading,
            IntroText: request.IntroText,
            StructuredContent: request.StructuredContent,
            ClosingText: request.ClosingText,
            CoverImageData: request.CoverImageData,
            CoverImageContentType: request.CoverImageContentType,
            BibleReference: request.BibleReference,
            AuthorName: request.AuthorName);

        var result = await mediator.Send(command, ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetPastorPostById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdatePastorPostRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdatePastorPostCommand(
            Id: id,
            Title: request.Title,
            Content: request.Content,
            Category: request.Category,
            ThemeOfTheYearId: request.ThemeOfTheYearId,
            Excerpt: request.Excerpt,
            IntroHeading: request.IntroHeading,
            IntroText: request.IntroText,
            StructuredContent: request.StructuredContent,
            ClosingText: request.ClosingText,
            CoverImageData: request.CoverImageData,
            CoverImageContentType: request.CoverImageContentType,
            BibleReference: request.BibleReference);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> PublishAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new PublishPastorPostCommand(id);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UnpublishAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UnpublishPastorPostCommand(id);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> PinAsync(
        Guid id,
        [FromBody] PinRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new PinPastorPostCommand(
            id,
            request.IsPinned);

        await mediator.Send(command, ct);

        return TypedResults.NoContent();
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeletePastorPostCommand(id);

        await mediator.Send(command, ct);

        return TypedResults.NoContent();
    }
}

// ============================================================
// API Request DTOs
// ============================================================

public record CreatePastorPostRequest(
    string Title,
    string Content,
    PostCategory Category,
    Guid ThemeOfTheYearId,
    string? Excerpt,
    string? IntroHeading,
    string? IntroText,
    StructuredContentDto? StructuredContent,
    string? ClosingText,
    byte[]? CoverImageData,
    string? CoverImageContentType,
    string? BibleReference,
    string AuthorName = "Pastor");

public record UpdatePastorPostRequest(
    string Title,
    string Content,
    PostCategory Category,
    Guid ThemeOfTheYearId,
    string? Excerpt,
    string? IntroHeading,
    string? IntroText,
    StructuredContentDto? StructuredContent,
    string? ClosingText,
    byte[]? CoverImageData,
    string? CoverImageContentType,
    string? BibleReference);

public record PinRequest(bool IsPinned);