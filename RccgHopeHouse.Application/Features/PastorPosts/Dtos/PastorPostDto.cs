using System.Text.Json;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PastorPosts.Dtos;

// ============================================================
// Structured content shapes (serialized into StructuredContentJson)
// ============================================================

public record FirstPointDto(
    string Title,
    string? Content,
    IReadOnlyList<string>? Bullets);

public record PrefaceSectionDto(
    string MainHeading,
    string Preamble,
    string SubHeading);

public record DetailedLessonDto(
    string Title,
    IReadOnlyList<string> Bullets);

public record StructuredContentDto(
    IReadOnlyList<FirstPointDto>? FirstPoints,
    PrefaceSectionDto? PrefaceSection,
    IReadOnlyList<DetailedLessonDto>? DetailedLessons)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string? Serialize(StructuredContentDto? content) =>
        content is null ? null : JsonSerializer.Serialize(content, JsonOptions);

    public static StructuredContentDto? Deserialize(string? json) =>
        string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<StructuredContentDto>(json, JsonOptions);
}

// ============================================================
// Full DTO — single-post views, admin editing
// ============================================================

/// <summary>
/// Detailed Data Transfer Object for a single Pastor's Corner post.
/// Used for admin views, single-post public views, and API responses.
/// </summary>
public record PastorPostDto(
    Guid Id,
    string Title,
    string Content,
    string? Excerpt,
    PostCategory Category,
    string? IntroHeading,
    string? IntroText,
    StructuredContentDto? StructuredContent,
    string? ClosingText,
    byte[]? CoverImageData,
    string? CoverImageContentType,
    string AuthorName,
    DateTime PublishedDate,
    bool IsPublished,
    bool IsPinned,
    bool IsFeatured,
    int ViewCount,
    string? BibleReference,
    Guid ThemeOfTheYearId,
    string? ThemeTitle)
{
    public static PastorPostDto FromEntity(PastorPost post) => new(
        Id: post.Id,
        Title: post.Title,
        Content: post.Content,
        Excerpt: post.Excerpt,
        Category: post.Category,
        IntroHeading: post.IntroHeading,
        IntroText: post.IntroText,
        StructuredContent: StructuredContentDto.Deserialize(post.StructuredContentJson),
        ClosingText: post.ClosingText,
        CoverImageData: post.CoverImageData,
        CoverImageContentType: post.CoverImageContentType,
        AuthorName: post.AuthorName,
        PublishedDate: post.PublishedDate,
        IsPublished: post.IsPublished,
        IsPinned: post.IsPinned,
        IsFeatured: post.IsFeatured,
        ViewCount: post.ViewCount,
        BibleReference: post.BibleReference,
        ThemeOfTheYearId: post.ThemeOfTheYearId,
        // ThemeTitle is only populated if the repository query included
        // ThemeOfTheYear (.Include(p => p.ThemeOfTheYear)) — otherwise null,
        // never throws.
        ThemeTitle: post.ThemeOfTheYear?.ThemeTitle);
}

// ============================================================
// Feed DTO — public scrolling feed, lightweight
// ============================================================

/// <summary>
/// Lightweight Data Transfer Object for the public scrolling feed.
/// </summary>
public record PastorPostFeedDto(
    Guid Id,
    string Title,
    string Excerpt,
    PostCategory Category,
    byte[]? CoverImageData,
    string AuthorName,
    DateTime PublishedDate,
    bool IsPinned,
    bool IsFeatured,
    Guid ThemeOfTheYearId,
    string? ThemeTitle)
{
    public static PastorPostFeedDto FromEntity(PastorPost post) => new(
        Id: post.Id,
        Title: post.Title,
        Excerpt: post.Excerpt ?? post.GenerateExcerpt(),
        Category: post.Category,
        CoverImageData: post.CoverImageData,
        AuthorName: post.AuthorName,
        PublishedDate: post.PublishedDate,
        IsPinned: post.IsPinned,
        IsFeatured: post.IsFeatured,
        ThemeOfTheYearId: post.ThemeOfTheYearId,
        ThemeTitle: post.ThemeOfTheYear?.ThemeTitle);
}