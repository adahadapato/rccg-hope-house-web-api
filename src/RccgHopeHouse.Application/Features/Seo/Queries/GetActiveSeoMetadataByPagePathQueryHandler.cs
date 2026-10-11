
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Queries;

/// <summary>
/// Handles public requests to retrieve active SEO metadata
/// associated with a website page path.
/// </summary>
public sealed class GetActiveSeoMetadataByPagePathQueryHandler
    : IRequestHandler<
        GetActiveSeoMetadataByPagePathQuery,
        SeoMetadataDto?>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the query handler.
    /// </summary>
    /// <param name="repository">
    /// Repository responsible for SEO metadata retrieval.
    /// </param>
    public GetActiveSeoMetadataByPagePathQueryHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves active SEO metadata for a public website path.
    /// </summary>
    /// <param name="request">
    /// Query containing the relative website path.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the database operation.
    /// </param>
    /// <returns>
    /// Active SEO metadata DTO, or null if no active
    /// metadata exists for the specified page.
    /// </returns>
    public async Task<SeoMetadataDto?> Handle(
        GetActiveSeoMetadataByPagePathQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.PagePath);

        var metadata = await _repository.GetActiveByPagePathAsync(
            request.PagePath.Trim(),
            cancellationToken);

        if (metadata is null)
        {
            return null;
        }

        // Map directly to the DTO.
        return new SeoMetadataDto(
            metadata.Id,
            metadata.ContentType,
            metadata.ContentId,
            metadata.ContentTitle,
            metadata.PagePath,
            metadata.SeoTitle,
            metadata.MetaDescription,
            metadata.Keywords,
            metadata.IsActive);
    }
}
