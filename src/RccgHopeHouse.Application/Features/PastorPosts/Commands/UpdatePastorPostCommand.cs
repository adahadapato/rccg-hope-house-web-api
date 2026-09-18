using MediatR;
using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PastorPosts.Commands;

/// <summary>
/// Command to update an existing Pastor's Corner post.
/// Supports metadata updates and cover image replacement.
/// </summary>
public record UpdatePastorPostCommand(
    Guid Id,
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
    string? BibleReference) : IRequest<PastorPostDto>;