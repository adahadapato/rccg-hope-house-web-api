using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

/// <summary>
/// Requests the annual prayer currently active
/// for display on the public website.
/// </summary>
public sealed record GetActiveAnnualPrayerQuery
    : IRequest<AnnualPrayerDto?>;