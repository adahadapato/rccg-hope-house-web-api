using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Handler for fetching filtered, paginated prayer requests.
/// </summary>
public class GetPrayerRequestsQueryHandler : IRequestHandler<GetPrayerRequestsQuery, IReadOnlyList<PrayerRequestDto>>
{
    private readonly IPrayerRequestRepository _repository;

    public GetPrayerRequestsQueryHandler(IPrayerRequestRepository repository) => _repository = repository;

    /// <summary>
    /// Fetches requests from repository and maps to DTOs.
    /// Orders by CreatedAt descending (newest first).
    /// </summary>
    public async Task<IReadOnlyList<PrayerRequestDto>> Handle(GetPrayerRequestsQuery request, CancellationToken ct)
    {
        var requests = await _repository.GetByStatusAsync(
            status: request.Status,
            skip: request.Skip,
            take: request.Take,
            ct: ct);

        return requests.Select(r => new PrayerRequestDto(
            r.Id,
            r.RequesterName,
            r.RequesterEmail,
            r.PhoneNumber?.Value,
            r.Content,
            r.Status,
            r.CreatedAt,
            null, // PastoralNote not loaded in list view for performance
            r.RespondedAt)).ToList();
    }
}