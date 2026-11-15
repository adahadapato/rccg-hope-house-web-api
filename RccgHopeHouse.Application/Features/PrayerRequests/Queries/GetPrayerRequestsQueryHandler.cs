using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Handles retrieval of paginated prayer requests for
/// the administration interface.
/// </summary>
public sealed class GetPrayerRequestsQueryHandler
    : IRequestHandler<
        GetPrayerRequestsQuery,
        IReadOnlyList<PrayerRequestDto>>
{
    private readonly IPrayerRequestRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetPrayerRequestsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve prayer requests.
    /// </param>
    public GetPrayerRequestsQueryHandler(
        IPrayerRequestRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves prayer requests using the requested status
    /// filter and pagination settings.
    /// </summary>
    /// <param name="request">
    /// The prayer request list query.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// A read-only collection of prayer request DTOs.
    /// </returns>
    public async Task<IReadOnlyList<PrayerRequestDto>> Handle(
        GetPrayerRequestsQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var skip =
            Math.Max(
                request.Skip,
                0);

        var take =
            Math.Clamp(
                request.Take,
                1,
                100);

        var requests =
            await _repository.GetByStatusAsync(
                status: request.Status,
                skip: skip,
                take: take,
                ct: ct);

        return requests
            .Select(
                PrayerRequestDto.FromEntity)
            .ToList();
    }
}