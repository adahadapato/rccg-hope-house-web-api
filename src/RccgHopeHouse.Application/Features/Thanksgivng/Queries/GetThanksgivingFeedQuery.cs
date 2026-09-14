using MediatR;
using RccgHopeHouse.Application.Features.Thanksgiving.Dtos;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Queries;

/// <summary>
/// Query to retrieve the public thanksgiving service feed.
/// Optimized for monthly chronological rendering with YouTube embed support.
/// </summary>
public record GetThanksgivingFeedQuery(
    int Skip = 0,
    int Take = 12) : IRequest<IReadOnlyList<ThanksgivingServiceDto>>;