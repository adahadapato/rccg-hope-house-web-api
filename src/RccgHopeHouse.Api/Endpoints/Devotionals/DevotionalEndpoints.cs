using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Devotionals.Commands;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Application.Features.Devotionals.Queries;

namespace RccgHopeHouse.Api.Endpoints.Devotionals;

/// <summary>
/// Minimal API endpoints for Daily Devotionals:
/// public devotional reading/history and admin CRUD.
/// </summary>
public static class DevotionalEndpoints
{
    public static RouteGroupBuilder MapDevotionalEndpoints(
        this RouteGroupBuilder group)
    {
        var devotionals = group
            .MapGroup("/devotionals")
            .WithTags("Daily Devotionals");

        // ============================================================
        // Public Endpoints
        // ============================================================

        devotionals.MapGet("/latest", GetLatestAsync)
            .WithSummary("Get latest public devotional")
            .WithDescription(
                "Returns today's published devotional when available. " +
                "Otherwise returns the most recent published devotional " +
                "dated before today.")
            .WithName("GetLatestDevotional")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        devotionals.MapGet("/date/{date}", GetByDateAsync)
            .WithSummary("Get public devotional by date")
            .WithDescription(
                "Returns a published devotional for the requested date. " +
                "Future devotionals are never exposed.")
            .WithName("GetDevotionalByDate")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        devotionals.MapGet("/history", GetHistoryAsync)
            .WithSummary("Get published devotional history")
            .WithDescription(
                "Returns paginated published devotionals dated today " +
                "or earlier, ordered by devotional date descending.")
            .WithName("GetDevotionalHistory")
            .Produces<IReadOnlyList<DevotionalHistoryDto>>(
                StatusCodes.Status200OK)
            .AllowAnonymous();

        // ============================================================
        // Admin Endpoints
        // ============================================================

        var admin = devotionals
            .MapGroup("/admin")
            .WithTags("Daily Devotionals - Admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapGet("/", GetAllForAdminAsync)
            .WithSummary("Get all devotionals for admin")
            .WithDescription(
                "Returns all devotionals for the admin management panel, " +
                "including drafts and future devotionals.")
            .WithName("GetDevotionalsForAdmin")
            .Produces<IReadOnlyList<DevotionalDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapGet("/{id:guid}", GetForAdminByIdAsync)
            .WithSummary("Get devotional by ID for admin")
            .WithDescription(
                "Returns a single devotional for admin viewing and editing.")
            .WithName("GetDevotionalForAdminById")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/", CreateAsync)
            .WithSummary("Create new devotional")
            .WithDescription(
                "Creates a devotional in draft status. " +
                "Future devotional dates are permitted.")
            .WithName("CreateDevotional")
            .Produces<DevotionalDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithSummary("Update existing devotional")
            .WithDescription(
                "Updates the date and content of an existing devotional.")
            .WithName("UpdateDevotional")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/publish", PublishAsync)
            .WithSummary("Publish devotional")
            .WithDescription(
                "Marks a devotional as published. " +
                "A future-dated devotional remains hidden from public " +
                "endpoints until its devotional date arrives.")
            .WithName("PublishDevotional")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapPost("/{id:guid}/unpublish", UnpublishAsync)
            .WithSummary("Unpublish devotional")
            .WithDescription(
                "Removes the devotional from public access while keeping " +
                "it available for editing and later republishing.")
            .WithName("UnpublishDevotional")
            .Produces<DevotionalDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithSummary("Delete devotional")
            .WithDescription(
                "Deletes the devotional from the system.")
            .WithName("DeleteDevotional")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return group;
    }

    // ============================================================
    // Public Handlers
    // ============================================================

    private static async Task<IResult> GetLatestAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var today = GetUkToday();

        var query = new GetLatestDevotionalQuery(
            Today: today);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetByDateAsync(
        DateOnly date,
        IMediator mediator,
        CancellationToken ct)
    {
        var today = GetUkToday();

        var query = new GetDevotionalByDateQuery(
            DevotionalDate: date,
            Today: today);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetHistoryAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        var today = GetUkToday();

        var query = new GetDevotionalHistoryQuery(
            Today: today,
            Skip: skip,
            Take: take);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    // ============================================================
    // Admin Handlers
    // ============================================================

    private static async Task<IResult> GetAllForAdminAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetDevotionalsForAdminQuery();

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetForAdminByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetDevotionalForAdminByIdQuery(id);

        var result = await mediator.Send(query, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateDevotionalRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new CreateDevotionalCommand(
            DevotionalDate: request.DevotionalDate,
            Theme: request.Theme,
            ScriptureReference: request.ScriptureReference,
            PassageId: request.PassageId,
            Thought: request.Thought,
            CommentaryPoints: request.CommentaryPoints,
            PrayerPoints: request.PrayerPoints,
            Declaration: request.Declaration);

        var result = await mediator.Send(command, ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetDevotionalForAdminById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateDevotionalRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UpdateDevotionalCommand(
            Id: id,
            DevotionalDate: request.DevotionalDate,
            Theme: request.Theme,
            ScriptureReference: request.ScriptureReference,
            PassageId: request.PassageId,
            Thought: request.Thought,
            CommentaryPoints: request.CommentaryPoints,
            PrayerPoints: request.PrayerPoints,
            Declaration: request.Declaration);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> PublishAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new PublishDevotionalCommand(id);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> UnpublishAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new UnpublishDevotionalCommand(id);

        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new DeleteDevotionalCommand(id);

        await mediator.Send(command, ct);

        return TypedResults.NoContent();
    }

    // ============================================================
    // Date Helpers
    // ============================================================

    /// <summary>
    /// Returns the current calendar date in the United Kingdom.
    /// Handles GMT and British Summer Time automatically.
    /// </summary>
    private static DateOnly GetUkToday()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
            "Europe/London");

        var ukNow = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            timeZone);

        return DateOnly.FromDateTime(ukNow);
    }
}

// ============================================================
// API Request DTOs
// ============================================================

public record CreateDevotionalRequest(
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference,
    string PassageId,
    string Thought,
    IReadOnlyList<string> CommentaryPoints,
    IReadOnlyList<string> PrayerPoints,
    string Declaration);

public record UpdateDevotionalRequest(
    DateOnly DevotionalDate,
    string Theme,
    string ScriptureReference,
    string PassageId,
    string Thought,
    IReadOnlyList<string> CommentaryPoints,
    IReadOnlyList<string> PrayerPoints,
    string Declaration);