using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

public class GetPrayerRequestByIdQueryHandler : IRequestHandler<GetPrayerRequestByIdQuery, PrayerRequestDto>
{
    private readonly IPrayerRequestRepository _repository;

    public GetPrayerRequestByIdQueryHandler(IPrayerRequestRepository repository) => _repository = repository;

    public async Task<PrayerRequestDto> Handle(GetPrayerRequestByIdQuery request, CancellationToken ct)
    {
        var prayer = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(PrayerRequest), request.Id);

        return PrayerRequestDto.FromEntity(prayer);
    }
}