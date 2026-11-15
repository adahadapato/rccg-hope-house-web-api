
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles publication of administrator-approved celebrations.
/// </summary>
/// <remarks>
/// Publication is permitted only when the celebration has been approved
/// by an administrator and its celebration date has been reached.
///
/// The handler records publication status in the database.
/// Public website queries must separately enforce publication visibility.
/// </remarks>
public sealed class PublishCelebrationCommandHandler : IRequestHandler<PublishCelebrationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes a new instance of the publication handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve and update celebrations.</param>
    public PublishCelebrationCommandHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Publishes an approved celebration after its celebration date has been reached.
    /// </summary>
    /// <param name="request">The publication command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The published celebration DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when the celebration identifier is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the celebration cannot be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the celebration cannot yet be published.</exception>
    public async Task<CelebrationDto> Handle(PublishCelebrationCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
            throw new ArgumentException("A valid celebration ID is required.", nameof(request.CelebrationId));

        var celebration = await _celebrationRepository.GetByIdAsync(request.CelebrationId, ct);

        if (celebration is null)
            throw new KeyNotFoundException("The selected celebration does not exist.");

        if (celebration.Status != "ReadyToPublish" || !celebration.ApprovedByAdminAt.HasValue)
            throw new InvalidOperationException("Administrator approval is required before publication.");

        var ukTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
        var ukNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ukTimeZone);
        var today = DateOnly.FromDateTime(ukNow);

        if (celebration.CelebrationDate > today)
            throw new InvalidOperationException("The celebration cannot be published before its scheduled date.");

        celebration.MarkPublished(DateTime.UtcNow);

        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return CelebrationDto.FromEntity(celebration);
    }
}
