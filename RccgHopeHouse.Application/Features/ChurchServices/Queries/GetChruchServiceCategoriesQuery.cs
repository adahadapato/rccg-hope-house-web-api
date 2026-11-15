using MediatR;

namespace RccgHopeHouse.Application.Features.ChurchServices.Queries;

/// <summary>
/// Requests all available church service categories.
/// </summary>
public sealed record GetChruchServiceCategoriesQuery
    : IRequest<IReadOnlyList<string>>;