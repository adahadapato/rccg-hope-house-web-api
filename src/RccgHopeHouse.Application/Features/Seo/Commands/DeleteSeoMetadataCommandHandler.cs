
using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles deletion of SEO metadata records.
/// </summary>
public sealed class DeleteSeoMetadataCommandHandler
    : IRequestHandler<DeleteSeoMetadataCommand, bool>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the deletion handler.
    /// </summary>
    /// <param name="repository">
    /// Repository responsible for SEO metadata persistence.
    /// </param>
    public DeleteSeoMetadataCommandHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Deletes an existing SEO metadata record.
    /// </summary>
    /// <param name="request">
    /// Command containing the SEO metadata identifier.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the database operation.
    /// </param>
    /// <returns>
    /// True if the record was deleted; otherwise false
    /// if the record was not found.
    /// </returns>
    public async Task<bool> Handle(
        DeleteSeoMetadataCommand request,
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

        await _repository.DeleteAsync(
            metadata,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}
