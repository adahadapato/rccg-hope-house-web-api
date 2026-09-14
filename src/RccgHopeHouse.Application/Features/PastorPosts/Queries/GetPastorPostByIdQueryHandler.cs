using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Queries;

/// <summary>
/// Handler for single post retrieval.
/// </summary>
public class GetPastorPostByIdQueryHandler : IRequestHandler<GetPastorPostByIdQuery, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public GetPastorPostByIdQueryHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Fetches published post, increments view count asynchronously, returns DTO.
    /// </summary>
    public async Task<PastorPostDto> Handle(GetPastorPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetPublishedByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        // Track engagement (fire-and-forget to avoid blocking response)
        _ = Task.Run(async () =>
        {
            post.IncrementViewCount();
            await _repository.UpdateAsync(post, CancellationToken.None);
            await _repository.SaveChangesAsync(CancellationToken.None);
        }, cancellationToken);

        return new PastorPostDto(
            post.Id, post.Title, post.Content, post.Excerpt, post.Category,
            post.CoverImageData, post.CoverImageContentType, post.AuthorName,
            post.PublishedDate, post.IsPublished, post.IsPinned, post.IsFeatured,
            post.ViewCount, post.BibleReference, post.Theme);
    }
}