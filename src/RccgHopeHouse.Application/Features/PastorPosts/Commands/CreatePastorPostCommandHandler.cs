using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// MediatR handler for CreatePastorPostCommand.
/// Orchestrates domain creation, validation, and persistence.
/// </summary>
public class CreatePastorPostCommandHandler : IRequestHandler<CreatePastorPostCommand, PastorPostDto>
{
    private readonly IPastorPostRepository _repository;

    /// <summary>
    /// Initializes a new instance of the handler.
    /// </summary>
    /// <param name="repository">Repository for pastor post data access.</param>
    public CreatePastorPostCommandHandler(IPastorPostRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the create command, enforcing domain rules and persisting the post.
    /// </summary>
    /// <param name="request">The create command containing post data.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A PastorPostDto representing the newly created post.</returns>
    public async Task<PastorPostDto> Handle(CreatePastorPostCommand request, CancellationToken cancellationToken)
    {
        // Auto-generate excerpt if not provided
        var excerpt = request.Excerpt;
        if (string.IsNullOrWhiteSpace(excerpt))
        {
            // Create temporary entity to use domain method
            var tempPost = PastorPost.Create(
                request.Title, request.Content, request.Category, request.AuthorName);
            excerpt = tempPost.GenerateExcerpt(150);
        }

        // Domain factory creation
        var post = PastorPost.Create(
            title: request.Title,
            content: request.Content,
            category: request.Category,
            authorName: request.AuthorName,
            excerpt: excerpt,
            coverImageData: request.CoverImageData,
            coverImageContentType: request.CoverImageContentType,
            bibleReference: request.BibleReference,
            theme: request.Theme);

        // Persist via repository
        await _repository.AddAsync(post, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Map to response DTO
        return MapToDto(post);
    }

    /// <summary>
    /// Maps a domain entity to a response DTO.
    /// </summary>
    private static PastorPostDto MapToDto(PastorPost post) => new(
        post.Id, post.Title, post.Content, post.Excerpt, post.Category,
        post.CoverImageData, post.CoverImageContentType, post.AuthorName,
        post.PublishedDate, post.IsPublished, post.IsPinned, post.IsFeatured,
        post.ViewCount, post.BibleReference, post.Theme);
}