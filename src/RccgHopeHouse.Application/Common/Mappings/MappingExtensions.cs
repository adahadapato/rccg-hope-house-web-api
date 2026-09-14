using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Common.Mappings;

/// <summary>
/// Extension methods for mapping domain entities to application DTOs.
/// Keeps mapping logic centralized and reusable across handlers.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Maps a PastorPost entity to a detailed DTO (admin/single-post view).
    /// Includes full content and cover image binary data.
    /// </summary>
    public static PastorPostDto ToPastorPostDto(this PastorPost post) => new(
        post.Id,
        post.Title,
        post.Content,
        post.Excerpt,
        post.Category,
        post.CoverImageData,
        post.CoverImageContentType,
        post.AuthorName,
        post.PublishedDate,
        post.IsPublished,
        post.IsPinned,
        post.IsFeatured,
        post.ViewCount,
        post.BibleReference,
        post.Theme);

    /// <summary>
    /// Maps a PastorPost entity to a lightweight feed DTO (public scrolling view).
    /// Excludes heavy binary data and full content for performance.
    /// </summary>
    public static PastorPostFeedDto ToPastorPostFeedDto(this PastorPost post) => new(
        post.Id,
        post.Title,
        post.Excerpt ?? post.GenerateExcerpt(150),
        post.Category,
        post.CoverImageData, // Repository should return compressed thumbnails only
        post.AuthorName,
        post.PublishedDate,
        post.IsPinned,
        post.IsFeatured,
        post.Theme);

    /// <summary>
    /// Maps a collection of PastorPost entities to feed DTOs.
    /// </summary>
    public static IReadOnlyList<PastorPostFeedDto> ToPastorPostFeedDtos(
        this IEnumerable<PastorPost> posts) =>
        posts.Select(ToPastorPostFeedDto).ToList();
}