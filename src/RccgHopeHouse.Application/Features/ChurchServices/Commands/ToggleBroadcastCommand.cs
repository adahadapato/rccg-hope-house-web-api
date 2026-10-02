using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Command to toggle whether broadcasts are enabled for a church service.
/// Broadcast-disabled services remain available as church services but are
/// excluded from broadcast functionality.
/// </summary>
public record ToggleBroadcastCommand(Guid Id)
    : IRequest<ChurchServiceDto>;