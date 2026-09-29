using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

/// <summary>
/// Query to retrieve published devotional history.
/// Only devotionals dated on or before the public visibility
/// boundary may be returned.
/// </summary>
public record GetDevotionalHistoryQuery(
    DateOnly Today,
    int Skip = 0,
    int Take = 30)
    : IRequest<IReadOnlyList<DevotionalHistoryDto>>;