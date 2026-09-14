using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Handler for updating a pastor post.
/// </summary>
public class UpdatePastorPostCommandHandler : IRequestHandler<UpdatePastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    public UpdatePastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Loads the existing post, applies updates, and persists changes.
    /// </summary>
    public async Task<PastorPostDto> Handle(UpdatePastorPostCommand request, CancellationToken cancellationToken)
    {
        // Load existing post for admin editing
        var post = await _repository.GetByIdForAdminAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(PastorPost), request.Id);

        // Apply domain update method
        post.Update(
            title: request.Title,
            content: request.Content,
            category: request.Category,
            excerpt: request.Excerpt,
            bibleReference: request.BibleReference,
            theme: request.Theme);

        // Update cover image if new data provided
        if (request.CoverImageData is not null)
        {
            post.SetCoverImage(request.CoverImageData, request.CoverImageContentType);
        }

        await _repository.UpdateAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(post);
    }

    private static PastorPostDto MapToDto(PastorPost post) => new(
        post.Id, post.Title, post.Content, post.Excerpt, post.Category,
        post.CoverImageData, post.CoverImageContentType, post.AuthorName,
        post.PublishedDate, post.IsPublished, post.IsPinned, post.IsFeatured,
        post.ViewCount, post.BibleReference, post.Theme);
}