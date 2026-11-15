using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Handles requests for filtered and paginated church service schedules.
/// </summary>
/// <remarks>
/// The handler retrieves church services from the repository using the
/// optional filters supplied by <see cref="GetChurchServicesQuery"/>
/// and maps the resulting domain entities to lightweight
/// <see cref="ChurchServiceFeedDto"/> objects for public consumption.
///
/// The feed includes scheduling information, presentation settings,
/// broadcast availability and the theme for the upcoming or currently
/// occurring instance of each service.
/// </remarks>
public sealed class GetChurchServicesQueryHandler
    : IRequestHandler<
        GetChurchServicesQuery,
        IReadOnlyList<ChurchServiceFeedDto>>
{
    private readonly IChurchServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetChurchServicesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve church service records.
    /// </param>
    public GetChurchServicesQueryHandler(
        IChurchServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves church services matching the supplied filters and
    /// maps them to public church service feed DTOs.
    /// </summary>
    /// <param name="request">
    /// The query containing the optional category, day, active-state,
    /// locality and pagination filters.
    /// </param>
    /// <param name="ct">
    /// Token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read-only collection of church service feed DTOs.
    /// </returns>
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
                        service.ShowInMonthlyServices,
                    IsBroadcastEnabled:
                        service.IsBroadcastEnabled,
                    CurrentTheme:
                        service.CurrentTheme))
            .ToList();
    }
}