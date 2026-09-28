using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

/// <summary>
/// Handler for retrieving filtered,
/// paginated church events.
/// </summary>
public class GetChurchEventsQueryHandler
    : IRequestHandler<
        GetChurchEventsQuery,
        IReadOnlyList<ChurchEventDto>>
{
    private readonly IChurchEventRepository _repository;

    public GetChurchEventsQueryHandler(
        IChurchEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ChurchEventDto>> Handle(
        GetChurchEventsQuery request,
        CancellationToken ct)
    {
        var events =
            await _repository.GetAllAsync(
                category: request.Category,
                isActive: request.IsActive,
                skip: request.Skip,
                take: request.Take,
                ct: ct);

        return events
            .Select(churchEvent =>
                new ChurchEventDto(
                    Id: churchEvent.Id,
                    Title: churchEvent.Title,
                    Category: churchEvent.Category,
                    StartDateTime:
                        churchEvent.StartDateTime,
                    EndDateTime:
                        churchEvent.EndDateTime,
                    Description:
                        churchEvent.Description,
                    Location:
                        churchEvent.Location,
                    Icon:
                        churchEvent.Icon,
                    Color:
                        churchEvent.Color,
                    RegistrationUrl:
                        churchEvent.RegistrationUrl,
                    RegistrationButtonText:
                        churchEvent.RegistrationButtonText,
                    ImageUrl:
                        churchEvent.ImageUrl,
                    IsActive:
                        churchEvent.IsActive,
                    DisplayOrder:
                        churchEvent.DisplayOrder))
            .ToList();
    }
}