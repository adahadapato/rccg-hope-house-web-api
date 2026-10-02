using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ContactUs.Commands;
using RccgHopeHouse.Application.Features.ContactUs.Dtos;
using RccgHopeHouse.Application.Features.ContactUs.Queries;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Api.Endpoints.ContactUs;

/// <summary>
/// Defines Minimal API endpoints for public Contact Us submissions
/// and administrative contact-inbox management.
/// </summary>
/// <remarks>
/// The public endpoint accepts Contact Us submissions.
///
/// Administrative endpoints require the <c>RequireAdmin</c>
/// authorization policy and provide operations for listing, viewing,
/// marking as read, replying to and deleting Contact Us submissions.
/// </remarks>
public static class ContactEndpoints
{
    /// <summary>
    /// Maps Contact Us endpoints to the supplied API route group.
    /// </summary>
    /// <param name="group">
    /// Parent API route group.
    /// </param>
    /// <returns>
    /// The supplied parent route group.
    /// </returns>
    public static RouteGroupBuilder MapContactUsEndpoints(
        this RouteGroupBuilder group)
    {
        var contact =
            group.MapGroup("/contact")
                .WithTags("Contact Us");

        // ==================== Public Endpoint ====================

        contact.MapPost(
                "/",
                SubmitAsync)
            .WithName("SubmitContactUs")
            .WithSummary("Submit contact form")
            .WithDescription(
                "Stores a public Contact Us submission, notifies the " +
                "RCCG Hope House Info mailbox and sends an acknowledgement " +
                "to the requester.")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .AllowAnonymous();

        // ==================== Admin Endpoints ====================

        var admin =
            contact.MapGroup("/admin")
                .RequireAuthorization(
                    "RequireAdmin");

        admin.MapGet(
                "/",
                GetListAsync)
            .WithName("GetContactUs")
            .WithSummary(
                "Get paginated contact requests for admin inbox")
            .Produces<IReadOnlyList<ContactUsDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapGet(
                "/unread",
                GetUnreadAsync)
            .WithName("GetUnreadContactUs")
            .WithSummary(
                "Get paginated unread contact requests")
            .Produces<IReadOnlyList<ContactUsDto>>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapGet(
                "/{id:guid}",
                GetByIdAsync)
            .WithName("GetContactUsById")
            .WithSummary(
                "Get a single contact request for review")
            .Produces<ContactUsDto>(
                StatusCodes.Status200OK)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost(
                "/{id:guid}/mark-read",
                MarkAsReadAsync)
            .WithName("MarkContactUsAsRead")
            .WithSummary(
                "Mark a contact request as read")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapPost(
                "/{id:guid}/reply",
                ReplyAsync)
            .WithName("ReplyToContactUs")
            .WithSummary(
                "Send a reply from the Info mailbox")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        admin.MapDelete(
                "/{id:guid}",
                DeleteAsync)
            .WithName("DeleteContactUs")
            .WithSummary(
                "Permanently delete a contact request")
            .Produces(
                StatusCodes.Status204NoContent)
            .ProducesProblem(
                StatusCodes.Status404NotFound)
            .ProducesProblem(
                StatusCodes.Status401Unauthorized);

        return group;
    }

    // ==================== Public Handler ====================

    /// <summary>
    /// Processes a public Contact Us form submission.
    /// </summary>
    private static async Task<IResult> SubmitAsync(
        [FromBody] SubmitContactRequestRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new SubmitContactUsCommand(
                FirstName: request.FirstName,
                LastName: request.LastName,
                Email: request.Email,
                Message: request.Message,
                Reason: request.Reason,
                PhoneNumber: request.PhoneNumber);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }

    // ==================== Admin Handlers ====================

    /// <summary>
    /// Retrieves Contact Us submissions for the administrative inbox.
    /// </summary>
    /// <remarks>
    /// When query-string pagination values are omitted, the endpoint
    /// defaults to the first 20 records.
    /// </remarks>
    private static async Task<IResult> GetListAsync(
        [FromQuery] bool? unreadOnly,
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetContactUsQuery(
                UnreadOnly:
                    unreadOnly ?? false,
                Skip:
                    skip ?? 0,
                Take:
                    take ?? 20);

        var list =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            list);
    }

    /// <summary>
    /// Retrieves unread Contact Us submissions for the
    /// administrative inbox.
    /// </summary>
    /// <remarks>
    /// When query-string pagination values are omitted, the endpoint
    /// defaults to the first 20 unread records.
    /// </remarks>
    private static async Task<IResult> GetUnreadAsync(
        [FromQuery] int? skip,
        [FromQuery] int? take,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetContactUsQuery(
                UnreadOnly: true,
                Skip:
                    skip ?? 0,
                Take:
                    take ?? 20);

        var list =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            list);
    }

    /// <summary>
    /// Retrieves an individual Contact Us submission.
    /// </summary>
    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query =
            new GetContactUsByIdQuery(
                id);

        var contact =
            await mediator.Send(
                query,
                cancellationToken);

        return TypedResults.Ok(
            contact);
    }

    /// <summary>
    /// Marks an individual Contact Us submission as read.
    /// </summary>
    private static async Task<IResult> MarkAsReadAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new MarkContactUsAsReadCommand(
                id);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Sends an administrator reply to the original requester.
    /// </summary>
    private static async Task<IResult> ReplyAsync(
        Guid id,
        [FromBody] ReplyToContactUsRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new ReplyToContactUsCommand(
                id,
                request.Subject,
                request.Body);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Permanently deletes an individual Contact Us submission.
    /// </summary>
    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command =
            new DeleteContactUsCommand(
                id);

        await mediator.Send(
            command,
            cancellationToken);

        return TypedResults.NoContent();
    }
}

/// <summary>
/// Request model for a public Contact Us form submission.
/// </summary>
/// <param name="FirstName">
/// Requester's first name.
/// </param>
/// <param name="LastName">
/// Requester's last name.
/// </param>
/// <param name="Email">
/// Requester's email address.
/// </param>
/// <param name="Message">
/// Message submitted by the requester.
/// </param>
/// <param name="Reason">
/// Reason for contacting RCCG Hope House.
/// </param>
/// <param name="PhoneNumber">
/// Optional contact telephone number.
/// </param>
public record SubmitContactRequestRequest(
    string FirstName,
    string LastName,
    string Email,
    string Message,
    ContactReason Reason,
    string? PhoneNumber);

/// <summary>
/// Request model for an administrator's reply to a
/// Contact Us submission.
/// </summary>
/// <param name="Subject">
/// Subject of the outgoing reply.
/// </param>
/// <param name="Body">
/// Reply message to send to the requester.
/// </param>
public record ReplyToContactUsRequest(
    string Subject,
    string Body);