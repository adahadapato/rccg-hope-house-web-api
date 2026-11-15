using MediatR;


using RccgHopeHouse.Application.Features.Seo.Dtos;


/// <summary>
/// Retrieves approved SEO metadata for a public website page.
/// </summary>
/// <param name="PagePath">
/// Relative URL path, such as /about or /contact.
/// </param>
public sealed record GetActiveSeoMetadataByPagePathQuery(
    string PagePath
) : IRequest<SeoMetadataDto?>;