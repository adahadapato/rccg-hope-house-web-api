using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Fetches other published posts/topics taught under the same ThemeOfTheYear
/// as the given post — powers the "other topics under this theme" popup on
/// a post's detail view. Excludes the post itself from the results.
/// </summary>
public record GetPastorPostSiblingsQuery(Guid PostId) : IRequest<IReadOnlyList<PastorPostFeedDto>>;