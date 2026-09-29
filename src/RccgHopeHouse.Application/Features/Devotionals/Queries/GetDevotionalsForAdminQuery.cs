using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

/// <summary>
/// Query to retrieve all devotionals for admin management.
/// Includes drafts and future devotionals.
/// </summary>
public record GetDevotionalsForAdminQuery
    : IRequest<IReadOnlyList<DevotionalDto>>;