using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Handler for single prayer request retrieval.
/// </summary>
public class GetPrayerRequestByIdQueryHandler : IRequestHandler<GetPrayerRequestByIdQuery, PrayerRequestDto>
{
    private readonly IPrayerRequestRepository _repository;

    public GetPrayerRequestByIdQueryHandler(IPrayerRequestRepository repository) => _repository = repository;

    /// <summary>
    /// Fetches the request by ID and maps to detailed DTO.
    /// </summary>
    public async Task<PrayerRequestDto> Handle(GetPrayerRequestByIdQuery request, CancellationToken ct)
    {
        var prayer = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(PrayerRequest), request.Id);

        return new PrayerRequestDto(
            prayer.Id,
            prayer.RequesterName,
            prayer.RequesterEmail,
            prayer.PhoneNumber?.Value, // ← Extract string from VO for serialization
            prayer.Content,
            prayer.Status,
            prayer.CreatedAt,
            prayer.PastoralNote,
            prayer.RespondedAt);

        // In GetPrayerRequestsQueryHandler & GetPrayerRequestByIdQueryHandler

    }
}