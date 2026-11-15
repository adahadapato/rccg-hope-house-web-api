using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Query to retrieve all posts for admin management panel.
/// Includes drafts and full metadata for editing.
/// </summary>
public record GetPastorPostsForAdminQuery(bool IncludeDrafts = true) : IRequest<IReadOnlyList<PastorPostDto>>;
