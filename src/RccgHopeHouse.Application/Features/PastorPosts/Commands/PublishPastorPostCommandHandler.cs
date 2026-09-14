using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Handler for publishing a post.
/// </summary>
public class PublishPastorPostCommandHandler : IRequestHandler<PublishPastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public PublishPastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Loads the post, calls domain Publish() method, and persists.
    /// </summary>
    public async Task<PastorPostDto> Handle(PublishPastorPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        post.Publish();

        await _repository.UpdateAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new PastorPostDto(
            post.Id, post.Title, post.Content, post.Excerpt, post.Category,
            post.CoverImageData, post.CoverImageContentType, post.AuthorName,
            post.PublishedDate, post.IsPublished, post.IsPinned, post.IsFeatured,
            post.ViewCount, post.BibleReference, post.Theme);
    }
}