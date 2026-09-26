using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

public record ActivateProphecyCommand(
    Guid Id)
    : IRequest<ProphecyDto>;