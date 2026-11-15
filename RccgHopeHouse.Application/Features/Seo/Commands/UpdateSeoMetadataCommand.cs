
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents a request to update an existing SEO metadata record.
/// </summary>
/// <param name="Id">
/// Unique identifier of the SEO metadata record.
/// </param>
/// <param name="SeoTitle">
/// Updated SEO title, maximum 70 characters.
/// </param>
/// <param name="MetaDescription">
/// Updated meta description, maximum 160 characters.
/// </param>
/// <param name="Keywords">
/// Optional comma-separated keywords, maximum 500 characters.
/// </param>
public sealed record UpdateSeoMetadataCommand(
    Guid Id,
    string SeoTitle,
    string MetaDescription,
    string? Keywords
) : IRequest<SeoMetadataDto?>;
