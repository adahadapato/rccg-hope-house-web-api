
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Seo.Queries;

/// <summary>
/// Represents a request to retrieve SEO metadata records
/// with optional pagination, filtering and searching.
/// </summary>
/// <param name="Page">Requested page number.</param>
/// <param name="PageSize">Number of records per page.</param>
/// <param name="ContentType">Optional content type filter.</param>
/// <param name="Search">Optional search term.</param>
public sealed record GetSeoMetadataQuery(
    int Page = 1,
    int PageSize = 20,
    string? ContentType = null,
    string? Search = null
) : IRequest<SeoPagedResult<SeoMetadataDto>>;
