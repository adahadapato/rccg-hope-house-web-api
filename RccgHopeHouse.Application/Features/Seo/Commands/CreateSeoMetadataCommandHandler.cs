
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles requests to create new SEO metadata records.
/// </summary>
public sealed class CreateSeoMetadataCommandHandler
    : IRequestHandler<CreateSeoMetadataCommand, SeoMetadataDto>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the handler with the SEO metadata repository.
    /// </summary>
    /// <param name="repository">
    /// Repository responsible for SEO metadata persistence.
    /// </param>
    public CreateSeoMetadataCommandHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Validates the request, prevents duplicate metadata,
    /// creates an inactive record and returns its DTO.
    /// </summary>
    /// <param name="request">The create command.</param>
    /// <param name="cancellationToken">
    /// Cancellation token for database operations.
    /// </param>
    /// <returns>The newly created SEO metadata.</returns>
    public async Task<SeoMetadataDto> Handle(
        CreateSeoMetadataCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ContentType);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            request.ContentTitle);

        if (request.ContentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Content ID cannot be an empty GUID.",
                nameof(request.ContentId));
        }

        if (!request.ContentId.HasValue &&
            string.IsNullOrWhiteSpace(request.PagePath))
        {
            throw new ArgumentException(
                "A content ID or page path is required.");
        }

        // Prevent duplicate metadata for the same content.
        if (request.ContentId.HasValue)
        {
            var existing = await _repository.GetByContentAsync(
                request.ContentType.Trim(),
                request.ContentId.Value,
                cancellationToken);

            if (existing is not null)
            {
                throw new InvalidOperationException(
                    "SEO metadata already exists for this content.");
            }
        }

        // Prevent duplicate metadata for the same page path.
        if (!string.IsNullOrWhiteSpace(request.PagePath))
        {
            var existing = await _repository.GetByPagePathAsync(
                request.PagePath.Trim(),
                cancellationToken);

            if (existing is not null)
            {
                throw new InvalidOperationException(
                    "SEO metadata already exists for this page path.");
            }
        }

        // The domain entity validates SEO field lengths
        // and creates the record with IsActive = false.
        var metadata = SeoMetadata.Create(
            request.ContentType,
            request.ContentId,
            request.ContentTitle,
            request.PagePath,
            request.SeoTitle,
            request.MetaDescription,
            request.Keywords);

        await _repository.AddAsync(
            metadata,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

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
