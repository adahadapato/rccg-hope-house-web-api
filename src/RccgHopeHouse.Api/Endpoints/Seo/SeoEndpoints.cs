
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Seo.Commands;
using RccgHopeHouse.Application.Features.Seo.Dtos;

namespace RccgHopeHouse.Api.Endpoints.Seo;

/// <summary>
/// API endpoints for generating SEO recommendations.
/// </summary>
public static class SeoEndpoints
{
    public static RouteGroupBuilder MapSeoEndpoints(this RouteGroupBuilder group)
    {
        var seo = group.MapGroup("/seo")
            .WithTags("SEO");

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

        return group;
    }

    private static async Task<IResult> GenerateSeoAsync(
        [FromBody] GenerateSeoRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["request"] = ["A request body is required."]
                });
        }

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.ContentType) ||
            !new[] { "Sermon", "Event", "Devotional", "Service" }
                .Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            errors["contentType"] =
                ["Content type must be Sermon, Event, Devotional or Service."];
        }

        if (string.IsNullOrWhiteSpace(request.Topic))
        {
            errors["topic"] = ["A topic is required."];
        }
        else if (request.Topic.Length > 200)
        {
            errors["topic"] = ["Topic cannot exceed 200 characters."];
        }

        if (request.Content?.Length > 15000)
        {
            errors["content"] = ["Content cannot exceed 15,000 characters."];
        }

        if (errors.Count > 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        try
        {
            var command = new GenerateSeoCommand(
                request.ContentType!,
                request.Topic!,
                request.Content);

            var result = await mediator.Send(
                command,
                cancellationToken);

            return TypedResults.Ok(result);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
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

    public sealed record GenerateSeoRequest(string? ContentType, string? Topic, string? Content);
}
