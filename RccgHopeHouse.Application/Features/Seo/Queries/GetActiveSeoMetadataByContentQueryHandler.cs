
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Queries;

/// <summary>
/// Handles public SEO metadata retrieval by content identifier.
/// </summary>
public sealed class GetActiveSeoMetadataByContentQueryHandler
    : IRequestHandler<
        GetActiveSeoMetadataByContentQuery,
        SeoMetadataDto?>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the public content SEO query handler.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve active SEO metadata.
    /// </param>
    public GetActiveSeoMetadataByContentQueryHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves active SEO metadata for the requested content.
    /// </summary>
    /// <param name="request">
    /// Query containing the content type and identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the database operation.
    /// </param>
    /// <returns>
    /// The active SEO metadata DTO, or null if no active
    /// metadata exists for the requested content.
    /// </returns>
    public async Task<SeoMetadataDto?> Handle(
        GetActiveSeoMetadataByContentQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ContentType);

        if (request.ContentId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid content ID is required.",
                nameof(request.ContentId));
        }

        var metadata =
            await _repository.GetActiveByContentAsync(
                request.ContentType.Trim(),
                request.ContentId,
                cancellationToken);

        if (metadata is null)
        {
            return null;
        }

        // Map directly to the DTO without a separate mapper.
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
