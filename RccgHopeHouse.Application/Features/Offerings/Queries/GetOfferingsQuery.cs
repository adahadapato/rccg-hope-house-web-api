using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;

namespace RccgHopeHouse.Application.Features.Offerings.Queries;

/// <summary>
/// Retrieves a paginated list of offerings for administration.
/// </summary>
public record GetOfferingsQuery(
    int Skip,
    int Take) : IRequest<IReadOnlyList<OfferingDto>>;