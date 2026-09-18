using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

public class GetPastorPostSiblingsQueryHandler
    : IRequestHandler<GetPastorPostSiblingsQuery, IReadOnlyList<PastorPostFeedDto>>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostSiblingsQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PastorPostFeedDto>> Handle(
        GetPastorPostSiblingsQuery request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetPublishedByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.PostId);

        var siblings = await _repository.GetPublishedByThemeAsync(
            post.ThemeOfTheYearId,
            excludePostId: post.Id,
            cancellationToken);

        return siblings.ToPastorPostFeedDtos();
    }
}