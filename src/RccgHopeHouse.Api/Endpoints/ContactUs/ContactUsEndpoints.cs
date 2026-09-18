using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ContactUs.Commands;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;
using RccgHopeHouse.Application.Features.ContactUs.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.ContactUs;

/// <summary>
/// Minimal API endpoints for contact form submissions and admin inbox management.
/// Public submission endpoint with async email notification; admin endpoints for review and response.
/// </summary>
public static class ContactEndpoints
{
    public static RouteGroupBuilder MapContactUsEndpoints(this RouteGroupBuilder group)
    {
        var contact = group.MapGroup("/contact")
                           .WithTags("Contact Us");

        // ===== Public Endpoint =====
        contact.MapPost("/", SubmitAsync)
               .WithName("SubmitContactUs")
               .WithSummary("Submit contact form")
               .WithDescription("Public form submission. Returns 204 No Content; admin is notified asynchronously.")
               .Produces(StatusCodes.Status204NoContent)
               .ProducesValidationProblem()
               .AllowAnonymous();

        // ===== Admin Endpoints =====
        var admin = contact.MapGroup("/admin")
                           .RequireAuthorization("RequireAdmin");

        admin.MapGet("/", GetListAsync)
             .WithName("GetContactUs")
             .WithSummary("Get paginated contact requests for admin inbox")
             .Produces<IReadOnlyList<ContactUsDto>>()
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapGet("/unread", GetUnreadAsync)
             .WithName("GetUnreadContactUs")
             .WithSummary("Get unread requests only")
             .Produces<IReadOnlyList<ContactUsDto>>()
             .ProducesProblem(StatusCodes.Status401Unauthorized);

        admin.MapGet("/{id:guid}", GetByIdAsync)
             .WithName("GetContactUsById")
             .WithSummary("Get single request for review")
             .Produces<ContactUsDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/mark-read", MarkAsReadAsync)
             .WithName("MarkContactUsAsRead")
             .WithSummary("Mark request as read in admin inbox")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapPost("/{id:guid}/reply", ReplyAsync)
             .WithName("ReplyToContactUs")
             .WithSummary("Send email reply to contact requester")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        admin.MapDelete("/{id:guid}", DeleteAsync)
             .WithName("DeleteContactUs")
             .WithSummary("Permanently delete contact request")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    // ===== Public Handler =====
    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitContactRequestRequest request,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new SubmitContactUsCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Email: request.Email,
            Message: request.Message,
            Reason: request.Reason,
            PhoneNumber: request.PhoneNumber);

        await mediator.Send(command, ct);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContactUsCommand(id);
        await mediator.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetContactUsByIdQuery(id);
        var contact = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(contact);
    }

    private static async Task<IResult> MarkAsReadAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new MarkContactUsAsReadCommand(id);
        await mediator.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> ReplyAsync(
        Guid id,
        [FromBody] ReplyToContactUsRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReplyToContactUsCommand(id, request.Subject, request.Body);
        await mediator.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }

    // ===== Admin Handlers =====
    private static async Task<IResult> GetListAsync(
        [FromQuery] bool unreadOnly,
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetContactUsQuery(
            UnreadOnly: unreadOnly,
            Skip: skip,
            Take: take);

        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetUnreadAsync(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetContactUsQuery(
            UnreadOnly: true,
            Skip: skip,
            Take: take);

        var list = await mediator.Send(query, cancellationToken);
        return TypedResults.Ok(list);
    }
}

// ==================== API Layer Request DTOs ====================
public record SubmitContactRequestRequest(
    string FirstName, string LastName, string Email, string Message,
    ContactReason Reason, string? PhoneNumber);

public record ReplyToContactUsRequest(string Subject, string Body);