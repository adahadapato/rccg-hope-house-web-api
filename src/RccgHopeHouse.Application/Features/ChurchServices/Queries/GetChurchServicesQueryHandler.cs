using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handler for fetching filtered, paginated service schedules.
/// </summary>
public class GetChurchServicesQueryHandler
    : IRequestHandler<
        GetChurchServicesQuery,
        IReadOnlyList<ChurchServiceFeedDto>>
{
    private readonly IChurchServiceRepository _repository;

    public GetChurchServicesQueryHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ChurchServiceFeedDto>> Handle(
        GetChurchServicesQuery request,
        CancellationToken ct)
    {
        var services =
            await _repository.GetAllAsync(
                category: request.Category,
                dayOfWeek: request.DayOfWeek,
                isActive: request.IsActive,
                isLocal: request.IsLocal,
                skip: request.Skip,
                take: request.Take,
                ct: ct);

        return services
            .Select(service =>
                new ChurchServiceFeedDto(
                    Id: service.Id,
                    Name: service.Name,
                    Category: service.Category,
                    DayOfWeek: service.DayOfWeek,
                    StartTime: service.StartTime,
                    EndTime: service.EndTime,
                    Location: service.Location,
                    ZoomId: service.ZoomId,
                    ZoomPasscode: service.ZoomPasscode,
                    IsLocal: service.IsLocal,
                    IsActive: service.IsActive,
                    Recurrence: service.Recurrence,
                    DayOfMonth: service.DayOfMonth,
                    DisplayOrder: service.DisplayOrder,
                    Icon: service.Icon,
                    ShowInMonthlyServices:
                        service.ShowInMonthlyServices))
            .ToList();
    }
}