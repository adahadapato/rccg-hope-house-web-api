
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Represents a request to retrieve an individual
/// celebration by its unique identifier.
/// </summary>
/// <param name="Id">
/// The unique identifier of the celebration.
/// </param>
/// <remarks>
/// Intended for authorised administrative use because
/// the returned DTO may contain member information,
/// consent status and publication history.
/// </remarks>
public sealed record GetCelebrationByIdQuery(
    Guid Id
) : IRequest<CelebrationDto?>;
