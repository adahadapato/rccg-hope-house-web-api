using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

/// <summary>
/// Gets the latest broadcast for Holy Communion, Holy Ghost Service, and
/// Thanksgiving Service in one call. Powers MonthlyServices.tsx.
/// </summary>
public record GetLatestServiceBroadcastsQuery : IRequest<IReadOnlyList<ServiceBroadcastDto>>;