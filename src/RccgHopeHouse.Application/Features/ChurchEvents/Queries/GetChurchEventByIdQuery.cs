using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Queries;

/// <summary>
/// Retrieves a single church event by ID
/// for administrative editing.
/// </summary>
public record GetChurchEventByIdQuery(
    Guid Id)
    : IRequest<ChurchEventDto>;