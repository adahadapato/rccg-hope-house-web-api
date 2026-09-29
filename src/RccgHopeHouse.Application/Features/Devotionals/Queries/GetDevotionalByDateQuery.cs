using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

/// <summary>
/// Query to retrieve a published devotional for a specific public date.
/// Future devotionals must not be exposed.
/// </summary>
public record GetDevotionalByDateQuery(
    DateOnly DevotionalDate,
    DateOnly Today) : IRequest<DevotionalDto>;