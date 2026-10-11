using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Seo.Commands;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Application.Features.Seo.Queries;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Api.Endpoints.Seo;

/// <summary>
/// Maps administrative and public SEO metadata endpoints.
/// </summary>
public static class SeoEndpoints
{
    /// <summary>
    /// Registers SEO endpoints within the parent API route group.
    /// </summary>
    public static RouteGroupBuilder MapSeoEndpoints(this RouteGroupBuilder group)
    {
        var seo = group.MapGroup("/seo").WithTags("SEO");
        var admin = seo.MapGroup("/admin")
            .RequireAuthorization("RequireAdmin")
            .RequireRateLimiting("Strict");

        admin.MapPost("/generate", GenerateSeoAsync)
            .WithName("GenerateSeo")
            .WithSummary("Generate SEO recommendations for website content")
            .Produces<SeoSuggestionDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status429TooManyRequests)
            .Produces(StatusCodes.Status502BadGateway);

        admin.MapPost("", CreateAsync)
            .WithName("CreateSeoMetadata")
            .Produces<SeoMetadataDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        admin.MapGet("", ListAsync)
            .WithName("ListSeoMetadata")
            .Produces<SeoPagedResult<SeoMetadataDto>>();

        admin.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetSeoMetadataById")
            .Produces<SeoMetadataDto>()
            .Produces(StatusCodes.Status404NotFound);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateSeoMetadata")
            .Produces<SeoMetadataDto>()
            .Produces(StatusCodes.Status404NotFound);

        admin.MapPatch("/{id:guid}/activate", ActivateAsync)
            .WithName("ActivateSeoMetadata")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        admin.MapPatch("/{id:guid}/deactivate", DeactivateAsync)
            .WithName("DeactivateSeoMetadata")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteSeoMetadata")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        seo.MapGet("/content/{contentType}/{contentId:guid}", GetActiveByContentAsync)
            .WithName("GetActiveSeoByContent")
            .Produces<SeoMetadataDto>()
            .Produces(StatusCodes.Status404NotFound);

        seo.MapGet("/page", GetActiveByPagePathAsync)
            .WithName("GetActiveSeoByPagePath")
            .Produces<SeoMetadataDto>()
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateSeoMetadataRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return ValidationError("request", "A request body is required.");

        if (string.IsNullOrWhiteSpace(request.ContentType) ||
            string.IsNullOrWhiteSpace(request.ContentTitle) ||
            string.IsNullOrWhiteSpace(request.SeoTitle) ||
            string.IsNullOrWhiteSpace(request.MetaDescription) ||
            (request.ContentId is null && string.IsNullOrWhiteSpace(request.PagePath)))
            return ValidationError("request", "Content type, content title, SEO title, description and either content ID or page path are required.");

        if (request.SeoTitle.Length > 70 || request.MetaDescription.Length > 160 || request.Keywords?.Length > 500)
            return ValidationError("request", "SEO title must be at most 70 characters, description 160, and keywords 500.");

        var result = await mediator.Send(new CreateSeoMetadataCommand(
            request.ContentType, request.ContentId, request.ContentTitle,
            request.PagePath, request.SeoTitle, request.MetaDescription,
            request.Keywords), cancellationToken);

        return TypedResults.Created($"/api/seo/admin/{result.Id}", result);
    }

    private static async Task<IResult> ListAsync(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? contentType = null,
        [FromQuery] string? search = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return ValidationError("pagination", "Page must be at least 1 and pageSize must be between 1 and 100.");

        var result = await mediator.Send(
            new GetSeoMetadataQuery(page, pageSize, contentType, search), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSeoMetadataByIdQuery(id), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id, [FromBody] UpdateSeoMetadataRequest request,
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.SeoTitle) ||
            string.IsNullOrWhiteSpace(request.MetaDescription))
            return ValidationError("request", "SEO title and meta description are required.");

        if (request.SeoTitle.Length > 70 || request.MetaDescription.Length > 160 || request.Keywords?.Length > 500)
            return ValidationError("request", "SEO title must be at most 70 characters, description 160, and keywords 500.");

        var result = await mediator.Send(new UpdateSeoMetadataCommand(
            id, request.SeoTitle, request.MetaDescription, request.Keywords), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<IResult> ActivateAsync(
        Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var succeeded = await mediator.Send(new ActivateSeoMetadataCommand(id), cancellationToken);
        return succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> DeactivateAsync(
        Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var succeeded = await mediator.Send(new DeactivateSeoMetadataCommand(id), cancellationToken);
        return succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> DeleteAsync(
        Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var succeeded = await mediator.Send(new DeleteSeoMetadataCommand(id), cancellationToken);
        return succeeded ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<IResult> GetActiveByContentAsync(
        string contentType, Guid contentId, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetActiveSeoMetadataByContentQuery(contentType, contentId), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<IResult> GetActiveByPagePathAsync(
        [FromQuery] string path, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path))
            return ValidationError("path", "A page path is required.");

        var result = await mediator.Send(
            new GetActiveSeoMetadataByPagePathQuery(path), cancellationToken);
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static IResult ValidationError(string field, string message) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]> { [field] = [message] });

    private static async Task<IResult> GenerateSeoAsync(
        [FromBody] GenerateSeoRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return ValidationError("request", "A request body is required.");

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.ContentType) ||
            !new[] { "Sermon", "Event", "Devotional", "Service" }
                .Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
            errors["contentType"] = ["Content type must be Sermon, Event, Devotional or Service."];

        if (string.IsNullOrWhiteSpace(request.Topic))
            errors["topic"] = ["A topic is required."];
        else if (request.Topic.Length > 200)
            errors["topic"] = ["Topic cannot exceed 200 characters."];

        if (request.Content?.Length > 15000)
            errors["content"] = ["Content cannot exceed 15,000 characters."];

        if (errors.Count > 0)
            return TypedResults.ValidationProblem(errors);

        try
        {
            var result = await mediator.Send(
                new GenerateSeoCommand(request.ContentType!, request.Topic!, request.Content),
                cancellationToken);
            return TypedResults.Ok(result);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException)
        {
            return TypedResults.Problem(
                title: "SEO generation unavailable",
                detail: "The SEO provider could not complete the request. Please try again later.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (System.Text.Json.JsonException)
        {
            return TypedResults.Problem(
                title: "Invalid SEO provider response",
                detail: "The SEO provider returned an invalid response. Please try again.",
                statusCode: StatusCodes.Status502BadGateway);
        }
        catch (InvalidOperationException)
        {
            return TypedResults.Problem(
                title: "SEO generation failed",
                detail: "SEO recommendations could not be generated. Please contact the administrator if the issue continues.",
                statusCode: StatusCodes.Status502BadGateway);
        }
    }

    /// <summary>Request body for AI-generated SEO suggestions.</summary>
    public sealed record GenerateSeoRequest(string? ContentType, string? Topic, string? Content);

    /// <summary>Request body for creating SEO metadata.</summary>
    public sealed record CreateSeoMetadataRequest(
        string? ContentType, Guid? ContentId, string? ContentTitle, string? PagePath,
        string? SeoTitle, string? MetaDescription, string? Keywords);

    /// <summary>Request body for updating editable SEO fields.</summary>
    public sealed record UpdateSeoMetadataRequest(
        string? SeoTitle, string? MetaDescription, string? Keywords);
}
