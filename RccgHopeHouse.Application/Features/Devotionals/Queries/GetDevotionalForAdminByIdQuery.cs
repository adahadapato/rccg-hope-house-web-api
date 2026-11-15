using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;

namespace RccgHopeHouse.Application.Features.Devotionals.Queries;

/// <summary>
/// Query to retrieve a single devotional for admin management.
/// May return a draft, published, past, current, or future devotional.
/// </summary>
public record GetDevotionalForAdminByIdQuery(
    Guid Id) : IRequest<DevotionalDto>;