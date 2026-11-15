
namespace RccgHopeHouse.Application.Features.Seo.Dtos;

/// <summary>
/// Represents stored SEO metadata returned by application queries.
/// </summary>
public sealed record SeoMetadataDto(
    Guid Id,
    string ContentType,
    Guid? ContentId,
    string ContentTitle,
    string? PagePath,
    string SeoTitle,
    string MetaDescription,
    string? Keywords,
    bool IsActive
);
