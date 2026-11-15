using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;

namespace RccgHopeHouse.Application.Features.Offerings.Queries;

/// <summary>
/// Retrieves an offering by its unique identifier.
/// </summary>
public record GetOfferingByIdQuery(Guid Id) : IRequest<OfferingDto>;