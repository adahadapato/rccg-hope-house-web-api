using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Announcements.Commands;
using RccgHopeHouse.Application.Features.Announcements.Dtos;
using RccgHopeHouse.Application.Features.Announcements.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.Announcements;

/// <summary>
/// News &amp; Updates: public floating-banner feed and admin management.
/// </summary>
[Authorize]
public static class AnnouncementEndpoints
{
    private const long MaxImageBytes = 10 * 1024 * 1024;

    public static RouteGroupBuilder MapAnnouncementEndpoints(
        this RouteGroupBuilder group)
    {
        var announcements = group
            .MapGroup("/announcements")
            .WithTags("News & Updates");

        // ===== Public =====

        announcements.MapGet("/live", GetLiveAsync)
            .WithName("GetLiveAnnouncements")
            .WithSummary("Get live announcements")
            .WithDescription(
                "Returns published announcements inside their display window. " +
                "Pass bannerOnly=true for the floating banner.")
            .Produces<IReadOnlyList<AnnouncementFeedDto>>(StatusCodes.Status200OK)
            .AllowAnonymous();

        // ===== Admin =====

        var admin = announcements
            .MapGroup("/admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapGet("/", GetAdminListAsync)
            .WithName("GetAnnouncements")
            .Produces<IReadOnlyList<AnnouncementDto>>(StatusCodes.Status200OK);

        admin.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetAnnouncementById")
            .Produces<AnnouncementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/", CreateAsync)
            .WithName("CreateAnnouncement")
            .Produces<AnnouncementDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        admin.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateAnnouncement")
            .Produces<AnnouncementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        admin.MapPost("/{id:guid}/toggle-published", TogglePublishedAsync)
            .WithName("ToggleAnnouncementPublished")
            .Produces<AnnouncementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/image", UploadImageAsync)
            .WithName("UploadAnnouncementImage")
            .Produces<AnnouncementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .ExcludeFromDescription();

        admin.MapDelete("/{id:guid}/image", RemoveImageAsync)
            .WithName("RemoveAnnouncementImage")
            .Produces<AnnouncementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteAnnouncement")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public handlers =====

    private static async Task<IResult> GetLiveAsync(
        [FromQuery] bool bannerOnly = false,
        [FromQuery] int take = 10,
        [FromServices] IMediator mediator = null!,
        CancellationToken ct = default)
    {
        var list = await mediator.Send(
            new GetLiveAnnouncementsQuery(bannerOnly, take), ct);

        return TypedResults.Ok(list);
    }

    // ===== Admin handlers =====

    private static async Task<IResult> GetAdminListAsync(
        [FromQuery] bool? isPublished,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        [FromServices] IMediator mediator = null!,
        CancellationToken ct = default)
    {
        var list = await mediator.Send(
            new GetAnnouncementsQuery(isPublished, skip, take), ct);

        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetAnnouncementByIdQuery(id), ct));
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] AnnouncementRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new CreateAnnouncementCommand(
                Title: request.Title,
                Type: request.Type,
                StartsAt: request.StartsAt,
                EndsAt: request.EndsAt,
                Summary: request.Summary,
                Body: request.Body,
                LinkUrl: request.LinkUrl,
                LinkText: request.LinkText,
                DisplayOrder: request.DisplayOrder,
                IsPublished: request.IsPublished,
                ShowInBanner: request.ShowInBanner,
                EventStartsAt: request.EventStartsAt,
                EventEndsAt: request.EventEndsAt,
                VenueName: request.VenueName,
                VenueAddress: request.VenueAddress),
            ct);

        return TypedResults.CreatedAtRoute(
            result,
            "GetAnnouncementById",
            new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] AnnouncementRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateAnnouncementCommand(
                Id: id,
                Title: request.Title,
                Type: request.Type,
                StartsAt: request.StartsAt,
                EndsAt: request.EndsAt,
                Summary: request.Summary,
                Body: request.Body,
                LinkUrl: request.LinkUrl,
                LinkText: request.LinkText,
                DisplayOrder: request.DisplayOrder,
                IsPublished: request.IsPublished,
                ShowInBanner: request.ShowInBanner,
                EventStartsAt: request.EventStartsAt,
                EventEndsAt: request.EventEndsAt,
                VenueName: request.VenueName,
                VenueAddress: request.VenueAddress),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> TogglePublishedAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new ToggleAnnouncementPublishedCommand(id), ct));
    }

    private static async Task<IResult> UploadImageAsync(
        Guid id,
        [FromForm] IFormFile file,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return TypedResults.BadRequest("An image file is required.");
        }

        if (file.Length > MaxImageBytes)
        {
            return TypedResults.BadRequest("Image cannot be larger than 10MB.");
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);

        var result = await mediator.Send(
            new SetAnnouncementImageCommand(id, ms.ToArray(), file.ContentType),
            ct);

        return TypedResults.Ok(result);
    }

    private static async Task<IResult> RemoveImageAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new RemoveAnnouncementImageCommand(id), ct));
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteAnnouncementCommand(id), ct);

        return TypedResults.NoContent();
    }
}

// ==================== API Request DTOs ====================

public record AnnouncementRequest(
    string Title,
    AnnouncementType Type = AnnouncementType.News,
    DateTime? StartsAt = null,
    DateTime? EndsAt = null,
    string? Summary = null,
    string? Body = null,
    string? LinkUrl = null,
    string? LinkText = null,
    int DisplayOrder = 0,
    bool IsPublished = true,
    bool ShowInBanner = true,
    DateTime? EventStartsAt = null,
    DateTime? EventEndsAt = null,
    string? VenueName = null,
    string? VenueAddress = null);
