using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

/// <summary>
/// Command used to update an individual prophecy statement.
/// </summary>
public record UpdateProphecyCommand(
    Guid Id,
    Guid CategoryId,
    string Text,
    int DisplayOrder)
    : IRequest<ProphecyDto>;