
using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles requests to deactivate SEO metadata records.
/// </summary>
public sealed class DeactivateSeoMetadataCommandHandler
    : IRequestHandler<DeactivateSeoMetadataCommand, bool>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes a new instance of the deactivation handler.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve and persist SEO metadata.
    /// </param>
    public DeactivateSeoMetadataCommandHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Deactivates an existing SEO metadata record.
    /// </summary>
    /// <param name="request">
    /// Command containing the SEO metadata identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the database operation.
    /// </param>
    /// <returns>
    /// True if the record exists and is inactive after
    /// processing; otherwise false.
    /// </returns>
    public async Task<bool> Handle(
        DeactivateSeoMetadataCommand request,
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
            return false;
        }

        // Already inactive: no database update required.
        if (!metadata.IsActive)
        {
            return true;
        }

        // Use the domain method to update the status
        // and modification timestamp.
        metadata.Deactivate();

        await _repository.UpdateAsync(
            metadata,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}
