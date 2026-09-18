using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

public class GetLatestServiceBroadcastsQueryHandler
    : IRequestHandler<GetLatestServiceBroadcastsQuery, IReadOnlyList<ServiceBroadcastDto>>
{
    private static readonly ServiceCategory[] BroadcastCategories =
    {
        ServiceCategory.HolyCommunion,
        ServiceCategory.HolyGhostService,
        ServiceCategory.ThanksgivingService
    };

    private readonly IServiceBroadcastRepository _repository;

    public GetLatestServiceBroadcastsQueryHandler(IServiceBroadcastRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<ServiceBroadcastDto>> Handle(
        GetLatestServiceBroadcastsQuery request, CancellationToken ct)
    {
        var broadcasts = await _repository.GetLatestForCategoriesAsync(BroadcastCategories, ct);
        return broadcasts.Select(ServiceBroadcastDto.FromEntity).ToList();
    }
}