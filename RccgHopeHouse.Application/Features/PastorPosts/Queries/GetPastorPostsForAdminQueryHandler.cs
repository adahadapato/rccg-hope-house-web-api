using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

public class GetPastorPostsForAdminQueryHandler : IRequestHandler<GetPastorPostsForAdminQuery, IReadOnlyList<PastorPostDto>>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostsForAdminQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PastorPostDto>> Handle(GetPastorPostsForAdminQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetAllForAdminAsync(request.IncludeDrafts, cancellationToken);
        return posts.Select(post => post.ToPastorPostDto()).ToList();
    }
}