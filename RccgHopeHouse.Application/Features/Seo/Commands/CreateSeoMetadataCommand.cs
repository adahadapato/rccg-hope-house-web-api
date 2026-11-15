
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents a request to create SEO metadata for
/// a sermon, service, event, devotional or standalone page.
/// </summary>
/// <remarks>
/// Newly created SEO metadata is initially inactive.
/// An administrator must approve and activate it
/// before it becomes publicly available.
/// </remarks>
/// <param name="ContentType">
/// Type of website content, such as Sermon,
/// Service, Event, Devotional or Page.
/// </param>
/// <param name="ContentId">
/// Identifier of the associated content.
/// Null for standalone pages.
/// </param>
/// <param name="ContentTitle">
/// Original title of the associated content.
/// </param>
/// <param name="PagePath">
/// Public relative URL path, if applicable.
/// </param>
/// <param name="SeoTitle">
/// Approved SEO page title.
/// </param>
/// <param name="MetaDescription">
/// Approved search engine description.
/// </param>
/// <param name="Keywords">
/// Optional comma-separated SEO keywords.
/// </param>
public sealed record CreateSeoMetadataCommand(
    string ContentType,
    Guid? ContentId,
    string ContentTitle,
    string? PagePath,
    string SeoTitle,
    string MetaDescription,
    string? Keywords
) : IRequest<SeoMetadataDto>;
