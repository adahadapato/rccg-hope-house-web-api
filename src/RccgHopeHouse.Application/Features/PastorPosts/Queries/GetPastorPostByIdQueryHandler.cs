using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

public class GetPastorPostByIdQueryHandler : IRequestHandler<GetPastorPostByIdQuery, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostByIdQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PastorPostDto> Handle(GetPastorPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetPublishedByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        _ = Task.Run(async () =>
        {
            post.IncrementViewCount();
            await _repository.UpdateAsync(post, CancellationToken.None);
            await _repository.SaveChangesAsync(CancellationToken.None);
        }, cancellationToken);

        return post.ToPastorPostDto();
    }
}