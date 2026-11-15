using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Handler for pinning/unpinning a post.
/// </summary>
public class PinPastorPostCommandHandler : IRequestHandler<PinPastorPostCommand, Unit>
{
    private readonly IPastorPostRepository _repository;

    public PinPastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Toggles pin state via domain methods and persists.
    /// </summary>
    public async Task<Unit> Handle(PinPastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        if (request.IsPinned)
            post.Pin();
        else
            post.Unpin();

        await _repository.UpdateAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}