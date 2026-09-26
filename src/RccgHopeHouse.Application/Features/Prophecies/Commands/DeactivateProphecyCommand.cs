using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public record DeactivateProphecyCommand(
    Guid Id)
    : IRequest<ProphecyDto>;