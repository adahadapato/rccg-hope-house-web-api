using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Handler for the public feed query.
/// </summary>
public class GetPastorPostFeedQueryHandler : IRequestHandler<GetPastorPostFeedQuery, IReadOnlyList<PastorPostFeedDto>>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostFeedQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Fetches paginated published posts and maps to lightweight feed DTOs.
    /// </summary>
    public async Task<IReadOnlyList<PastorPostFeedDto>> Handle(GetPastorPostFeedQuery request, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetPublishedFeedAsync(
            category: request.Category,
            skip: request.Skip,
            take: request.Take,
            ct: cancellationToken);

        return posts.ToPastorPostFeedDtos();
    }
}