using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

/// <summary>
/// Handler for the public Upcoming Events feed.
/// </summary>
public class GetUpcomingChurchEventsQueryHandler
    : IRequestHandler<
        GetUpcomingChurchEventsQuery,
        IReadOnlyList<ChurchEventFeedDto>>
{
    private readonly IChurchEventRepository _repository;

    public GetUpcomingChurchEventsQueryHandler(
        IChurchEventRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ChurchEventFeedDto>> Handle(
        GetUpcomingChurchEventsQuery request,
        CancellationToken ct)
    {
        var events =
            await _repository.GetUpcomingAsync(
                DateTime.UtcNow,
                request.Take,
                ct);

        return events
            .Select(churchEvent =>
                new ChurchEventFeedDto(
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
                    DisplayOrder:
                        churchEvent.DisplayOrder))
            .ToList();
    }
}