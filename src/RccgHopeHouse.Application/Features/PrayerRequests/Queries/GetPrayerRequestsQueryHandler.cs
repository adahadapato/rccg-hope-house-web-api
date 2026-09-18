using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

public class GetPrayerRequestsQueryHandler : IRequestHandler<GetPrayerRequestsQuery, IReadOnlyList<PrayerRequestDto>>
{
    private readonly IPrayerRequestRepository _repository;

    public GetPrayerRequestsQueryHandler(IPrayerRequestRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<PrayerRequestDto>> Handle(GetPrayerRequestsQuery request, CancellationToken ct)
    {
        var requests = await _repository.GetByStatusAsync(
            status: request.Status,
            skip: request.Skip,
            take: request.Take,
            ct: ct);

        return requests.Select(r => new PrayerRequestDto(
            Id: r.Id,
            RequesterName: r.RequesterName,
            IsAnonymous: r.IsAnonymous,
            RequesterEmail: r.RequesterEmail?.Value,
            PhoneNumber: r.PhoneNumber?.Value,
            Content: r.Content,
            Status: r.Status,
            CreatedAt: r.CreatedAt,
            PastoralNote: null, // not loaded in list view for performance
            RespondedAt: r.RespondedAt)).ToList();
    }
}