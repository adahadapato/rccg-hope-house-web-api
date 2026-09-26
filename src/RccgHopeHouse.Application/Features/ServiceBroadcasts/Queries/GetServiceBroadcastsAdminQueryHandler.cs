using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

public class GetServiceBroadcastsAdminQueryHandler
    : IRequestHandler<
        GetServiceBroadcastsAdminQuery,
        IReadOnlyList<ServiceBroadcastDto>>
{
    private readonly IServiceBroadcastRepository
        _repository;

    public GetServiceBroadcastsAdminQueryHandler(
        IServiceBroadcastRepository repository)
    {
        _repository = repository;
    }

    public async Task<
        IReadOnlyList<ServiceBroadcastDto>>
        Handle(
            GetServiceBroadcastsAdminQuery request,
            CancellationToken ct)
    {
        var skip =
            Math.Max(0, request.Skip);

        var take =
            Math.Clamp(
                request.Take,
                1,
                500);

        var broadcasts =
            await _repository.GetAllAsync(
                skip,
                take,
                ct);

        return broadcasts
            .Select(
                ServiceBroadcastDto.FromEntity)
            .ToList();
    }
}