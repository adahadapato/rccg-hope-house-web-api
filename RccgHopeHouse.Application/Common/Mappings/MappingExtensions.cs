using RccgHopeHouse.Application.Features.PastorPosts.Dtos;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Common.Mappings;

public static class MappingExtensions
{
    public static PastorPostDto ToPastorPostDto(this PastorPost post) => new(
        post.Id,
        post.Title,
        post.Content,
        post.Excerpt,
        post.Category,
        post.IntroHeading,
        post.IntroText,
        StructuredContentDto.Deserialize(post.StructuredContentJson),
        post.ClosingText,
        post.CoverImageData,
        post.CoverImageContentType,
        post.AuthorName,
        post.PublishedDate,
        post.IsPublished,
        post.IsPinned,
        post.IsFeatured,
        post.ViewCount,
        post.BibleReference,
        post.ThemeOfTheYearId,
        post.ThemeOfTheYear?.ThemeTitle);

    public static PastorPostFeedDto ToPastorPostFeedDto(this PastorPost post) => new(
        post.Id,
        post.Title,
        post.Excerpt ?? post.GenerateExcerpt(150),
        post.Category,
        post.CoverImageData, // TODO: repository should return compressed thumbnails only
        post.AuthorName,
        post.PublishedDate,
        post.IsPinned,
        post.IsFeatured,
        post.ThemeOfTheYearId,
        post.ThemeOfTheYear?.ThemeTitle);

    public static IReadOnlyList<PastorPostFeedDto> ToPastorPostFeedDtos(
        this IEnumerable<PastorPost> posts) =>
        posts.Select(ToPastorPostFeedDto).ToList();
}