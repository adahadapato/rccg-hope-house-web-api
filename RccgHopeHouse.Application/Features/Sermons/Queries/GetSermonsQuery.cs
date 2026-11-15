using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;

namespace RccgHopeHouse.Application.Features.Sermons.Queries;

/// <summary>
/// Query to retrieve a paginated list of published sermons with optional search and date filtering.
/// Designed for public-facing feeds and admin lists.
/// </summary>
public record GetSermonsQuery(
    string? Search = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Skip = 0,
    int Take = 20) : IRequest<IReadOnlyList<SermonDto>>;