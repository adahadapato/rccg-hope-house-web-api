using MediatR;
using RccgHopeHouse.Application.Common.Mappings;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Handles unpublishing a Pastor's Corner post.
/// The post remains in the database and can be edited or republished later.
/// </summary>
public class UnpublishPastorPostCommandHandler
    : IRequestHandler<UnpublishPastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public UnpublishPastorPostCommandHandler(
        IPastorPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PastorPostDto> Handle(
        UnpublishPastorPostCommand request,
        CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(PastorPost),
                request.Id);

        post.Unpublish();

        await _repository.UpdateAsync(
            post,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return post.ToPastorPostDto();
    }
}