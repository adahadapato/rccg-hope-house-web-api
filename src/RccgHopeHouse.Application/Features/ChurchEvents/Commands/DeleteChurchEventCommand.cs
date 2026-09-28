using MediatR;

namespace RccgHopeHouse.Application.Features.ChurchEvents.Commands;

public record DeleteChurchEventCommand(
    Guid Id)
    : IRequest;