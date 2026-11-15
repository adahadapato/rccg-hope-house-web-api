
using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles activation of approved SEO metadata.
/// </summary>
public sealed class ActivateSeoMetadataCommandHandler
    : IRequestHandler<ActivateSeoMetadataCommand, bool>
{
    private readonly ISeoMetadataRepository _repository;

    /// <summary>
    /// Initializes the activation handler.
    /// </summary>
    public ActivateSeoMetadataCommandHandler(
        ISeoMetadataRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Activates the requested SEO metadata record.
    /// </summary>
    public async Task<bool> Handle(
        ActivateSeoMetadataCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            throw new ArgumentException(
                "A valid SEO metadata ID is required.",
                nameof(request.Id));

        var metadata = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (metadata is null)
            return false;

        if (metadata.IsActive)
            return true;

        metadata.Activate();

        await _repository.UpdateAsync(
            metadata,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}
