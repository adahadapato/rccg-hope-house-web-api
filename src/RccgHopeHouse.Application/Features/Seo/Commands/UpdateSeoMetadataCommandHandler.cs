
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles requests to update existing SEO metadata records.
/// </summary>
public sealed class UpdateSeoMetadataCommandHandler
    : IRequestHandler<UpdateSeoMetadataCommand, SeoMetadataDto?>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the handler.
    /// </summary>
    /// <param name="repository">
    /// Repository responsible for SEO metadata persistence.
    /// </param>
    public UpdateSeoMetadataCommandHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Updates SEO metadata and returns the updated record.
    /// </summary>
    /// <param name="request">The update command.</param>
    /// <param name="cancellationToken">
    /// Cancellation token for database operations.
    /// </param>
    /// <returns>
    /// Updated SEO metadata, or null if the record does not exist.
    /// </returns>
    public async Task<SeoMetadataDto?> Handle(
        UpdateSeoMetadataCommand request,
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

        // Update through the domain entity.
        // This validates field lengths and updates the timestamp.
        metadata.UpdateMetadata(
            request.SeoTitle,
            request.MetaDescription,
            request.Keywords);

        await _repository.UpdateAsync(
            metadata,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        // Map directly without a separate mapper class.
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
