using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

/// <summary>
/// Query to retrieve the latest publicly available devotional.
/// Returns today's published devotional when available; otherwise
/// returns the most recent previously published devotional.
/// </summary>
public record GetLatestDevotionalQuery(
    DateOnly Today) : IRequest<DevotionalDto>;