using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Query to retrieve church services with optional filtering.
/// Optimized for public feeds: returns lightweight DTOs, excludes admin-only fields.
/// </summary>
public record GetChurchServicesQuery(
    ServiceCategory? Category = null,
    DayOfWeek? DayOfWeek = null,
    bool? IsActive = null,
    int Skip = 0,
    int Take = 20) : IRequest<IReadOnlyList<ChurchServiceFeedDto>>;