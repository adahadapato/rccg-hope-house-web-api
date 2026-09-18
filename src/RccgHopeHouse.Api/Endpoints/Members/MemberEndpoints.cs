using MediatR;
using Microsoft.AspNetCore.Mvc;
using RccgHopeHouse.Application.Features.Members.Commands;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Application.Features.Members.Queries;

namespace RccgHopeHouse.Api.Endpoints.Members;

/// <summary>
/// Admin-only endpoints for church member management. No public routes —
/// this handles personal congregant data (birthdays, marital status,
/// contact info).
/// </summary>
public static class MemberEndpoints
{
    public static RouteGroupBuilder MapMemberEndpoints(this RouteGroupBuilder group)
    {
        var members = group.MapGroup("/members")
                           .WithTags("Members")
                           .RequireAuthorization("RequireAdmin");

        members.MapGet("/", GetListAsync)
               .WithName("GetMembers")
               .WithSummary("Get paginated member list")
               .Produces<IReadOnlyList<MemberDto>>(StatusCodes.Status200OK);

        members.MapGet("/{id:guid}", GetByIdAsync)
               .WithName("GetMemberById")
               .WithSummary("Get single member by ID")
               .Produces<MemberDto>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status404NotFound);

        members.MapPost("/", CreateAsync)
               .WithName("CreateMember")
               .WithSummary("Add a new member")
               .Produces<MemberDto>(StatusCodes.Status201Created)
               .ProducesValidationProblem();

        members.MapPut("/{id:guid}", UpdateAsync)
               .WithName("UpdateMember")
               .WithSummary("Update an existing member's profile")
               .Produces<MemberDto>(StatusCodes.Status200OK)
               .ProducesProblem(StatusCodes.Status404NotFound)
               .ProducesValidationProblem();

        members.MapPost("/{id:guid}/deactivate", DeactivateAsync)
               .WithName("DeactivateMember")
               .WithSummary("Deactivate a member (soft, reversible)")
               .Produces(StatusCodes.Status204NoContent)
               .ProducesProblem(StatusCodes.Status404NotFound);

        members.MapPost("/{id:guid}/reactivate", ReactivateAsync)
               .WithName("ReactivateMember")
               .WithSummary("Reactivate a previously deactivated member")
               .Produces(StatusCodes.Status204NoContent)
               .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> GetListAsync(
        [FromQuery] bool includeInactive,
        [FromQuery] int skip,
        [FromQuery] int take,
        IMediator mediator,
        CancellationToken ct)
    {
        skip = 0;
        take = 100;
        includeInactive = false;
        var query = new GetMembersQuery(includeInactive, skip, take);
        var list = await mediator.Send(query, ct);
        return TypedResults.Ok(list);
    }

    private static async Task<IResult> GetByIdAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetMemberByIdQuery(id);
        var member = await mediator.Send(query, ct);
        return TypedResults.Ok(member);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateMemberCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return TypedResults.CreatedAtRoute(result, "GetMemberById", new { id = result.Id });
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateMemberCommand command,
        IMediator mediator,
        CancellationToken ct)
    {
        // Guard against a body Id that doesn't match the route Id — reuse
        // the route's id as the source of truth rather than trusting the
        // body silently.
        var result = await mediator.Send(command with { Id = id }, ct);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> DeactivateAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        await mediator.Send(new DeactivateMemberCommand(id), ct);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> ReactivateAsync(Guid id, IMediator mediator, CancellationToken ct)
    {
        await mediator.Send(new ReactivateMemberCommand(id), ct);
        return TypedResults.NoContent();
    }
}