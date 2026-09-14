using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Handler for deleting a post.
/// </summary>
public class DeletePastorPostCommandHandler : IRequestHandler<DeletePastorPostCommand, Unit>
{
    private readonly IPastorPostRepository _repository;

    public DeletePastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Loads and deletes the post, then persists changes.
    /// </summary>
    public async Task<Unit> Handle(DeletePastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        await _repository.DeleteAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}