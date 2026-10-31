
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Represents an administrative request to retrieve celebrations by workflow stage.
/// </summary>
/// <param name="Status">
/// Optional workflow filter: Upcoming, Pending, AwaitingMemberResponse,
/// ApprovedByMember, ReadyToPublish, or Published.
/// </param>
/// <param name="FromDate">
/// Optional inclusive start date for date-based queries.
/// Defaults to the current UK date.
/// </param>
/// <param name="ThroughDate">
/// Optional inclusive end date for date-based queries.
/// Defaults to 30 days after the effective start date.
/// </param>
/// <remarks>
/// When no status is provided, Upcoming is used.
///
/// Date ranges apply to Upcoming, Pending, and Published.
/// ReadyToPublish uses ThroughDate as its publication cutoff.
///
/// This query returns administrative information and must only
/// be exposed through authorised administrative endpoints.
/// </remarks>
public sealed record GetCelebrationsQuery(
    string? Status = null,
    DateOnly? FromDate = null,
    DateOnly? ThroughDate = null
) : IRequest<IReadOnlyList<CelebrationDto>>;
