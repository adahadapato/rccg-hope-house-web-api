using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.ChurchInfo.Commands;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Application.Features.ChurchInfo.Queries;

namespace RccgHopeHouse.Api.Endpoints.ChurchInfo;

/// <summary>
/// Minimal API endpoints for the church's contact/profile information.
/// Provides public read access and content-editor management operations.
/// </summary>
public static class ChurchInfoEndpoints
{
    /// <summary>
    /// Maps the church information endpoints.
    /// </summary>
    public static RouteGroupBuilder MapChurchInfoEndpoints(
        this RouteGroupBuilder group)
    {
        var churchInfo = group
            .MapGroup("/church-info")
            .WithTags("Church Info");

        // ==================== Public Endpoint ====================

        churchInfo.MapGet("/", GetAsync)
            .WithName("GetChurchInfo")
            .WithSummary(
                "Get church address, contact methods, and About-section content")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        // ==================== Admin Endpoints ====================

        var admin = churchInfo
            .MapGroup("/admin")
            .RequireAuthorization("RequireContentEditor");

        admin.MapPut("/address", UpdateAddressAsync)
            .WithName("UpdateChurchAddress")
            .WithSummary("Update the church's address")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPut("/about", UpdateAboutSectionAsync)
            .WithName("UpdateChurchAboutSection")
            .WithSummary("Update the church's About-section content")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPost(
                "/contact-methods",
                AddContactMethodAsync)
            .WithName("AddChurchContactMethod")
            .WithSummary(
                "Add a contact, website, messaging, or social media method")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapPut(
                "/contact-methods/{id:guid}",
                UpdateContactMethodAsync)
            .WithName("UpdateChurchContactMethod")
            .WithSummary(
                "Update a church contact, website, messaging, or social media method")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        admin.MapDelete(
                "/contact-methods/{id:guid}",
                RemoveContactMethodAsync)
            .WithName("RemoveChurchContactMethod")
            .WithSummary("Remove a church contact method")
            .Produces<ChurchInfoDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    /// <summary>
    /// Gets the church information.
    /// </summary>
    private static async Task<IResult> GetAsync(
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetChurchInfoQuery();
        var info = await mediator.Send(query, ct);

        return TypedResults.Ok(info);
    }

    /// <summary>
    /// Updates the church address.
    /// </summary>
    private static async Task<IResult> UpdateAddressAsync(
        [FromBody] UpdateChurchAddressCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Updates the church About-section content.
    /// </summary>
    private static async Task<IResult> UpdateAboutSectionAsync(
        [FromBody] UpdateChurchAboutSectionCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Adds a church contact method.
    /// </summary>
    private static async Task<IResult> AddContactMethodAsync(
        [FromBody] AddChurchContactMethodCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Updates a church contact method.
    /// </summary>
    private static async Task<IResult> UpdateContactMethodAsync(
        Guid id,
        [FromBody] UpdateChurchContactMethodCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var request = command with
        {
            Id = id
        };

        var result = await mediator.Send(request, ct);

        return TypedResults.Ok(result);
    }

    /// <summary>
    /// Removes a church contact method.
    /// </summary>
    private static async Task<IResult> RemoveContactMethodAsync(
        Guid id,
        IMediator mediator,
        CancellationToken ct)
    {
        var command = new RemoveChurchContactMethodCommand(id);
        var result = await mediator.Send(command, ct);

        return TypedResults.Ok(result);
    }
}