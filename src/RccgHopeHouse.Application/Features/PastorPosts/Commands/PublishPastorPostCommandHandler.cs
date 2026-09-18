using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

public class PublishPastorPostCommandHandler : IRequestHandler<PublishPastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public PublishPastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PastorPostDto> Handle(PublishPastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        post.Publish();

        await _repository.UpdateAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return post.ToPastorPostDto();
    }
}