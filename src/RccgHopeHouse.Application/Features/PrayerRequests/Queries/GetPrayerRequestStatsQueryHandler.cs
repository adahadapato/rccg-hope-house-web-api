using MediatR;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

public class GetPrayerRequestStatsQueryHandler : IRequestHandler<GetPrayerRequestStatsQuery, PrayerRequestStatsDto>
{
    private readonly IPrayerRequestRepository _repository;

    public GetPrayerRequestStatsQueryHandler(IPrayerRequestRepository repository) => _repository = repository;

    public async Task<PrayerRequestStatsDto> Handle(GetPrayerRequestStatsQuery request, CancellationToken ct)
    {
        var pending = await _repository.GetCountByStatusAsync(Core.Enums.PrayerRequestStatus.Pending, ct);
        var inProgress = await _repository.GetCountByStatusAsync(Core.Enums.PrayerRequestStatus.InProgress, ct);
        var resolved = await _repository.GetCountByStatusAsync(Core.Enums.PrayerRequestStatus.Resolved, ct);
        var total = pending + inProgress + resolved;

        return new PrayerRequestStatsDto(pending, inProgress, resolved, total);
    }
}