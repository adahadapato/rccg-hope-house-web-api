using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.BirthdayWishes.Commands;
using RccgHopeHouse.Application.Features.BirthdayWishes.Dtos;
using RccgHopeHouse.Application.Features.BirthdayWishes.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.BirthdayWishes;

/// <summary>
/// Birthday wishes on "Celebrants of the Month" photos:
/// public reading and sending, and admin moderation.
/// Wishes stay on the website; nothing is sent to members.
/// </summary>
[Authorize]
public static class BirthdayWishEndpoints
{
    public static RouteGroupBuilder MapBirthdayWishEndpoints(
        this RouteGroupBuilder group)
    {
        var wishes = group
            .MapGroup("/birthday-wishes")
            .WithTags("Birthday Wishes");

        // ===== Public =====

        wishes.MapGet("/image/{imageId:guid}", GetForImageAsync)
            .WithName("GetBirthdayWishesForImage")
            .WithSummary("Get approved birthday wishes for a celebrant photo")
            .Produces<IReadOnlyList<BirthdayWishDto>>(StatusCodes.Status200OK)
            .AllowAnonymous();

        wishes.MapGet("/counts", GetCountsAsync)
            .WithName("GetBirthdayWishCounts")
            .WithSummary("Get the number of approved wishes per photo")
            .Produces<IReadOnlyList<BirthdayWishCountDto>>(StatusCodes.Status200OK)
            .AllowAnonymous();

        wishes.MapPost("/", SubmitAsync)
            .WithName("SubmitBirthdayWish")
            .WithSummary("Send a birthday wish")
            .WithDescription(
                "The wish is checked before it is shown. The response says " +
                "whether it was published, is waiting for review, or was rejected.")
            .Produces<SubmitBirthdayWishResultDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous()
            .RequireRateLimiting("BirthdayWishSubmit");

        // ===== Admin =====

        var admin = wishes
            .MapGroup("/admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapGet("/", GetAdminListAsync)
            .WithName("GetAdminBirthdayWishes")
            .Produces<IReadOnlyList<AdminBirthdayWishDto>>(StatusCodes.Status200OK);

        admin.MapGet("/stats", GetStatsAsync)
            .WithName("GetBirthdayWishStats")
            .Produces<BirthdayWishStatsDto>(StatusCodes.Status200OK);

        admin.MapPost("/{id:guid}/approve", ApproveAsync)
            .WithName("ApproveBirthdayWish")
            .Produces<AdminBirthdayWishDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/reject", RejectAsync)
            .WithName("RejectBirthdayWish")
            .Produces<AdminBirthdayWishDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteBirthdayWish")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public handlers =====

    private static async Task<IResult> GetForImageAsync(
        Guid imageId,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetBirthdayWishesForImageQuery(imageId), ct));
    }

    private static async Task<IResult> GetCountsAsync(
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetBirthdayWishCountsQuery(), ct));
    }

    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitBirthdayWishRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        // Honeypot: the "website" box is hidden from people, so only
        // bots fill it in. Pretend it worked and save nothing.
        if (!string.IsNullOrWhiteSpace(request.Website))
        {
            return TypedResults.Ok(
                new SubmitBirthdayWishResultDto(
                    BirthdayWishStatus.Pending,
                    "Thank you! Your wish will appear once it has been checked.",
                    null));
        }

        var result = await mediator.Send(
            new SubmitBirthdayWishCommand(
                request.GalleryImageId,
                request.AuthorName ?? string.Empty,
                request.Message ?? string.Empty),
            ct);

        return TypedResults.Ok(result);
    }

    // ===== Admin handlers =====

    private static async Task<IResult> GetAdminListAsync(
        [FromQuery] BirthdayWishStatus? status,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromServices] IMediator mediator = null!,
        CancellationToken ct = default)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetAdminBirthdayWishesQuery(status, skip, take), ct));
    }

    private static async Task<IResult> GetStatsAsync(
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetBirthdayWishStatsQuery(), ct));
    }

    private static async Task<IResult> ApproveAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new SetBirthdayWishStatusCommand(id, BirthdayWishStatus.Approved),
                ct));
    }

    private static async Task<IResult> RejectAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new SetBirthdayWishStatusCommand(id, BirthdayWishStatus.Rejected),
                ct));
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteBirthdayWishCommand(id), ct);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

/// <summary>A visitor's birthday wish, as sent by the website.</summary>
/// <param name="GalleryImageId">The celebrant photo the wish is for.</param>
/// <param name="AuthorName">The visitor's name.</param>
/// <param name="Message">The wish (500 characters max).</param>
/// <param name="Website">
/// Honeypot field. Hidden on the website; must stay empty.
/// </param>
public record SubmitBirthdayWishRequest(
    Guid GalleryImageId,
    string? AuthorName,
    string? Message,
    string? Website = null);
