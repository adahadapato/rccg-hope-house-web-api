using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.AiPrompts.Commands;
using RccgHopeHouse.Application.Features.AiPrompts.Dtos;
using RccgHopeHouse.Application.Features.AiPrompts.Queries;

namespace RccgHopeHouse.Api.Endpoints.AiPrompts;

/// <summary>
/// AI Prompts admin page: view, edit, improve with AI, restore
/// and reset the prompts used by the website's AI features.
/// </summary>
[Authorize]
public static class AiPromptEndpoints
{
    public static RouteGroupBuilder MapAiPromptEndpoints(
        this RouteGroupBuilder group)
    {
        var prompts = group
            .MapGroup("/ai-prompts")
            .WithTags("AI Prompts")
            .RequireAuthorization("RequireContentEditor");

        prompts.MapGet("/", GetAllAsync)
            .WithName("GetAiPrompts")
            .WithSummary("List the editable AI prompts")
            .Produces<IReadOnlyList<AiPromptSummaryDto>>(StatusCodes.Status200OK);

        prompts.MapGet("/{key}", GetAsync)
            .WithName("GetAiPrompt")
            .WithSummary("Get a prompt with its default and history")
            .Produces<AiPromptDetailDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        prompts.MapPut("/{key}", SaveAsync)
            .WithName("SaveAiPrompt")
            .WithSummary("Save a new version of a prompt (used straight away)")
            .Produces<AiPromptDetailDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        prompts.MapPost("/{key}/restore/{versionId:guid}", RestoreAsync)
            .WithName("RestoreAiPromptVersion")
            .WithSummary("Make an earlier version active again")
            .Produces<AiPromptDetailDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        prompts.MapPost("/{key}/reset", ResetAsync)
            .WithName("ResetAiPrompt")
            .WithSummary("Go back to the built-in default")
            .Produces<AiPromptDetailDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        prompts.MapPost("/{key}/suggest", SuggestAsync)
            .WithName("SuggestAiPrompt")
            .WithSummary("Ask AI to rewrite the prompt (nothing is saved)")
            .Produces<AiPromptSuggestionDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireRateLimiting("AiPromptSuggest");

        return group;
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(await mediator.Send(new GetAiPromptsQuery(), ct));
    }

    private static async Task<IResult> GetAsync(
        string key,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(await mediator.Send(new GetAiPromptQuery(key), ct));
    }

    private static async Task<IResult> SaveAsync(
        string key,
        [FromBody] SaveAiPromptRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new SaveAiPromptCommand(key, request.Instructions ?? string.Empty, request.ChangeNote),
                ct));
    }

    private static async Task<IResult> RestoreAsync(
        string key,
        Guid versionId,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new RestoreAiPromptVersionCommand(key, versionId), ct));
    }

    private static async Task<IResult> ResetAsync(
        string key,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new ResetAiPromptCommand(key), ct));
    }

    private static async Task<IResult> SuggestAsync(
        string key,
        [FromBody] SuggestAiPromptRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new SuggestAiPromptCommand(
                    key,
                    request.CurrentInstructions ?? string.Empty,
                    request.RequestedChange ?? string.Empty),
                ct));
    }
}

// ==================== API Request DTOs ====================

/// <summary>A prompt edited on the AI Prompts page.</summary>
/// <param name="Instructions">The full prompt text.</param>
/// <param name="ChangeNote">Optional note on what changed and why.</param>
public record SaveAiPromptRequest(
    string? Instructions,
    string? ChangeNote = null);

/// <summary>A request for the AI to rewrite a prompt.</summary>
/// <param name="CurrentInstructions">The prompt as it is in the editor now.</param>
/// <param name="RequestedChange">What the admin wants changed.</param>
public record SuggestAiPromptRequest(
    string? CurrentInstructions,
    string? RequestedChange);
