
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.Seo.Queries;

/// <summary>
/// Handles requests to retrieve paginated SEO metadata
/// for the Smart SEO administration dashboard.
/// </summary>
public sealed class GetSeoMetadataQueryHandler
    : IRequestHandler<
        GetSeoMetadataQuery,
        SeoPagedResult<SeoMetadataDto>>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the query handler.
    /// </summary>
    public GetSeoMetadataQueryHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves filtered SEO metadata records and
    /// returns the requested page.
    /// </summary>
    public async Task<SeoPagedResult<SeoMetadataDto>> Handle(
        GetSeoMetadataQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var skip = checked((page - 1) * pageSize);

        var records = await _repository.GetPagedAsync(
            skip,
            pageSize,
            request.ContentType,
            request.Search,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            request.ContentType,
            request.Search,
            cancellationToken);

        var items = records.Select(metadata =>
            new SeoMetadataDto(
                metadata.Id,
                metadata.ContentType,
                metadata.ContentId,
                metadata.ContentTitle,
                metadata.PagePath,
                metadata.SeoTitle,
                metadata.MetaDescription,
                metadata.Keywords,
                metadata.IsActive
            )
        ).ToList();

        return new SeoPagedResult<SeoMetadataDto>(
            items,
            totalCount,
            page,
            pageSize);
    }
}
