using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

/// <summary>
/// Requests the list of annual prayers for the
/// administration interface.
/// </summary>
public sealed record GetAnnualPrayersListQuery
    : IRequest<IReadOnlyList<AnnualPrayerDto>>;