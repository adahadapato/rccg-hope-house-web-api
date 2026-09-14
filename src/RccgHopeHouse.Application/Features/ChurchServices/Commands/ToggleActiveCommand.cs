using MediatR;
using RccgHopeHouse.Application.Features.ChurchServices.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Command to toggle a service's active status.
/// Inactive services are hidden from public feeds but remain in admin views.
/// </summary>
public record ToggleActiveCommand(Guid Id) : IRequest<ChurchServiceDto>;