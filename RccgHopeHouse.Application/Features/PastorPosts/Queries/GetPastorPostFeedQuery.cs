using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Query to retrieve the public scrolling feed.
/// Optimized for performance: returns lightweight DTOs, excludes full content.
/// Orders by: Pinned posts first, then by PublishedDate (descending).
/// </summary>
public record GetPastorPostFeedQuery(
    PostCategory? Category = null,
    int Skip = 0,
    int Take = 10) : IRequest<IReadOnlyList<PastorPostFeedDto>>;
