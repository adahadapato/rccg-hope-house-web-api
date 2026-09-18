using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to create a new Pastor's Corner post.
/// Creates the post as a draft by default. Use PublishPastorPostCommand to make it public.
/// </summary>
public record CreatePastorPostCommand(
    string Title,
    string Content,
    PostCategory Category,
    Guid ThemeOfTheYearId,
    string? Excerpt,
    string? IntroHeading,
    string? IntroText,
    StructuredContentDto? StructuredContent,
    string? ClosingText,
    byte[]? CoverImageData,
    string? CoverImageContentType,
    string? BibleReference,
    string AuthorName = "Pastor") : IRequest<PastorPostDto>;