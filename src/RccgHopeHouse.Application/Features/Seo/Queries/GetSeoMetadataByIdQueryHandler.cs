
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Queries;

/// <summary>
/// Handles requests to retrieve an SEO metadata record
/// by its unique identifier.
/// </summary>
public sealed class GetSeoMetadataByIdQueryHandler
    : IRequestHandler<GetSeoMetadataByIdQuery, SeoMetadataDto?>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the query handler.
    /// </summary>
    /// <param name="repository">
    /// Repository responsible for SEO metadata retrieval.
    /// </param>
    public GetSeoMetadataByIdQueryHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves an SEO metadata record and returns its DTO.
    /// </summary>
    /// <param name="request">
    /// Query containing the SEO metadata identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the database operation.
    /// </param>
    /// <returns>
    /// SEO metadata DTO, or null if the record does not exist.
    /// </returns>
    public async Task<SeoMetadataDto?> Handle(
        GetSeoMetadataByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid SEO metadata ID is required.",
                nameof(request.Id));
        }

        var metadata = await _repository.GetByIdAsync(
            request.Id,
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
