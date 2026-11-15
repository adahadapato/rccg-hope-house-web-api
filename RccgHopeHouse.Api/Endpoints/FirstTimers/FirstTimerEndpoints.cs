using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.FirstTimers.Commands;
using RccgHopeHouse.Application.Features.FirstTimers.Dtos;
using RccgHopeHouse.Application.Features.FirstTimers.Queries;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.FirstTimers;

public static class FirstTimerEndpoints
{
    public static RouteGroupBuilder MapFirstTimerEndpoints(
        this RouteGroupBuilder group)
    {
        var firstTimers = group
            .MapGroup("/first-timers")
            .WithTags("First Timers");

        // ===== Public (the form behind the QR code) =====

        firstTimers.MapPost("/", SubmitAsync)
            .WithName("SubmitFirstTimer")
            .WithSummary("Submit the first-time visitor welcome form")
            .WithDescription(
                "Saves the visitor as a first-timer. A prayer request, if given, " +
                "is saved as a confidential prayer request for the prayer team.")
            .Produces<SubmitFirstTimerResultDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .RequireRateLimiting("FirstTimerSubmit");

        // ===== Admin =====

        var admin = firstTimers
            .MapGroup("/admin")
            .RequireAuthorization(Policies.RequireFirstTimerTeam);

        admin.MapGet("/", GetListAsync)
            .WithName("GetFirstTimers")
            .WithSummary("List first-time visitors, newest first")
            .Produces<IReadOnlyList<FirstTimerDto>>(StatusCodes.Status200OK);

        admin.MapGet("/stats", GetStatsAsync)
            .WithName("GetFirstTimerStats")
            .Produces<FirstTimerStatsDto>(StatusCodes.Status200OK);

        admin.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetFirstTimerById")
            .Produces<FirstTimerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPut("/{id:guid}/follow-up", UpdateFollowUpAsync)
            .WithName("UpdateFirstTimerFollowUp")
            .WithSummary("Update the follow-up status and note")
            .Produces<FirstTimerDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // Creating members and deleting records is for Admins only,
        // the same as the members list itself.

        admin.MapPost("/{id:guid}/promote", PromoteAsync)
            .WithName("PromoteFirstTimerToMember")
            .WithSummary("Add the visitor to the members list")
            .Produces<PromoteFirstTimerResultDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Policies.RequireAdmin);

        admin.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteFirstTimer")
            .WithSummary("Permanently delete a first-timer record")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(Policies.RequireAdmin);

        return group;
    }

    // ===== Public handler =====

    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitFirstTimerRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        // Honeypot: the "website" box is hidden from people, so only
        // bots fill it in. Pretend it worked and save nothing.
        if (!string.IsNullOrWhiteSpace(request.Website))
        {
            return TypedResults.Ok(
                new SubmitFirstTimerResultDto(
                    "Thank you for worshipping with us today. We are so glad you came!"));
        }

        var result = await mediator.Send(
            new SubmitFirstTimerCommand(
                FirstName: request.FirstName ?? string.Empty,
                LastName: request.LastName ?? string.Empty,
                PhoneNumber: request.PhoneNumber ?? string.Empty,
                Email: request.Email,
                AddressLine1: request.AddressLine1,
                AddressLine2: request.AddressLine2,
                City: request.City,
                Postcode: request.Postcode,
                BirthMonth: request.BirthMonth,
                BirthDay: request.BirthDay,
                Source: request.Source,
                InvitedBy: request.InvitedBy,
                WantsPastoralContact: request.WantsPastoralContact,
                PreferredContactMethod:
                    request.PreferredContactMethod ?? FirstTimerContactMethod.PhoneCall,
                PrayerRequest: request.PrayerRequest,
                Comment: request.Comment,
                ConsentToContact: request.ConsentToContact),
            ct);

        return TypedResults.Ok(result);
    }

    // ===== Admin handlers =====

    private static async Task<IResult> GetListAsync(
        [FromQuery] FirstTimerFollowUpStatus? status,
        [FromQuery] string? search,
        [FromServices] IMediator mediator,
        CancellationToken ct,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new GetFirstTimersQuery(status, search, skip, take),
                ct));
    }

    private static async Task<IResult> GetStatsAsync(
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetFirstTimerStatsQuery(), ct));
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(new GetFirstTimerByIdQuery(id), ct));
    }

    private static async Task<IResult> UpdateFollowUpAsync(
        Guid id,
        [FromBody] UpdateFirstTimerFollowUpRequest request,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        return TypedResults.Ok(
            await mediator.Send(
                new UpdateFirstTimerFollowUpCommand(id, request.Status, request.Note),
                ct));
    }

    private static async Task<IResult> PromoteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct,
        [FromQuery] bool force = false)
    {
        return TypedResults.Ok(
            await mediator.Send(new PromoteFirstTimerCommand(id, force), ct));
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken ct)
    {
        await mediator.Send(new DeleteFirstTimerCommand(id), ct);

        return TypedResults.NoContent();
    }
}

/// <summary>
/// Body of the public welcome form. <c>Website</c> is the honeypot
/// and must stay empty.
/// </summary>
public record SubmitFirstTimerRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? BirthMonth,
    int? BirthDay,
    FirstTimerSource? Source,
    string? InvitedBy,
    bool WantsPastoralContact,
    FirstTimerContactMethod? PreferredContactMethod,
    string? PrayerRequest,
    string? Comment,
    bool ConsentToContact,
    string? Website);

public record UpdateFirstTimerFollowUpRequest(
    FirstTimerFollowUpStatus Status,
    string? Note);
