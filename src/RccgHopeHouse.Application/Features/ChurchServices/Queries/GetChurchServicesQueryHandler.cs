using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handler for fetching filtered, paginated service schedules.
/// </summary>
public class GetChurchServicesQueryHandler : IRequestHandler<GetChurchServicesQuery, IReadOnlyList<ChurchServiceFeedDto>>
{
    private readonly IChurchServiceRepository _repository;

    public GetChurchServicesQueryHandler(IChurchServiceRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<ChurchServiceFeedDto>> Handle(GetChurchServicesQuery request, CancellationToken ct)
    {
        var services = await _repository.GetAllAsync(
            category: request.Category,
            dayOfWeek: request.DayOfWeek,
            isActive: request.IsActive,
            skip: request.Skip,
            take: request.Take,
            ct: ct);

        return services.Select(s => new ChurchServiceFeedDto(
            s.Id, s.Name, s.Category, s.DayOfWeek, s.StartTime, s.EndTime,
            s.Location, s.ZoomId, s.ZoomPasscode, s.IsActive)).ToList();
    }
}