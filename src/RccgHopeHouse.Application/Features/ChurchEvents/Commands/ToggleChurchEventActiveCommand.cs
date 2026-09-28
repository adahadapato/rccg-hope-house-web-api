using MediatR;
using RccgHopeHouse.Application.Features.ChurchEvents.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Commands;

public record ToggleChurchEventActiveCommand(
    Guid Id)
    : IRequest<ChurchEventDto>;