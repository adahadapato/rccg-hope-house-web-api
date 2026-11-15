using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

/// <summary>
/// Requests a specific annual prayer by identifier.
/// </summary>
public sealed record GetAnnualPrayerByIdQuery(
    Guid Id)
    : IRequest<AnnualPrayerDto?>;