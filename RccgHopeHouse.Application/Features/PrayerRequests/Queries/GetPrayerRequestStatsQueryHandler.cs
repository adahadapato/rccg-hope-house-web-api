using MediatR;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Handles retrieval of prayer request workflow statistics.
/// </summary>
public sealed class GetPrayerRequestStatsQueryHandler
    : IRequestHandler<
        GetPrayerRequestStatsQuery,
        PrayerRequestStatsDto>
{
    private readonly IPrayerRequestRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetPrayerRequestStatsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve prayer request counts.
    /// </param>
    public GetPrayerRequestStatsQueryHandler(
        IPrayerRequestRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves prayer request counts for each workflow status
    /// and calculates the overall total.
    /// </summary>
    /// <param name="request">
    /// The statistics query.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Prayer request workflow statistics.
    /// </returns>
    public async Task<PrayerRequestStatsDto> Handle(
        GetPrayerRequestStatsQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pending =
            await _repository.GetCountByStatusAsync(
                PrayerRequestStatus.Pending,
                ct);

        var inProgress =
            await _repository.GetCountByStatusAsync(
                PrayerRequestStatus.InProgress,
                ct);

        var resolved =
            await _repository.GetCountByStatusAsync(
                PrayerRequestStatus.Resolved,
                ct);

        var total =
            pending +
            inProgress +
            resolved;

        return new PrayerRequestStatsDto(
            PendingCount: pending,
            InProgressCount: inProgress,
            ResolvedCount: resolved,
            TotalCount: total);
    }
}