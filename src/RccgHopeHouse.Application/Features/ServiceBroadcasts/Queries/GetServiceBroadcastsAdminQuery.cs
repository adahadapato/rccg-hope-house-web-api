using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

/// <summary>
/// Gets service broadcast history for administration.
/// </summary>
public record GetServiceBroadcastsAdminQuery(
    int Skip = 0,
    int Take = 100
) : IRequest<IReadOnlyList<ServiceBroadcastDto>>;