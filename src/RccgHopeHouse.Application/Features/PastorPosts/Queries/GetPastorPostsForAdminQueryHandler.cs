using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Handler for admin list query.
/// </summary>
public class GetPastorPostsForAdminQueryHandler : IRequestHandler<GetPastorPostsForAdminQuery, IReadOnlyList<PastorPostDto>>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostsForAdminQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Fetches posts for admin view and maps to full DTOs.
    /// </summary>
    public async Task<IReadOnlyList<PastorPostDto>> Handle(GetPastorPostsForAdminQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetAllForAdminAsync(request.IncludeDrafts, cancellationToken);

        return posts.Select(post => new PastorPostDto(
            post.Id, post.Title, post.Content, post.Excerpt, post.Category,
            post.CoverImageData, post.CoverImageContentType, post.AuthorName,
            post.PublishedDate, post.IsPublished, post.IsPinned, post.IsFeatured,
            post.ViewCount, post.BibleReference, post.Theme)).ToList();
    }
}