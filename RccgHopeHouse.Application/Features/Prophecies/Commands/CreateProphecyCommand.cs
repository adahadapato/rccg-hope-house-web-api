using MediatR;
using RccgHopeHouse.Application.Features.Prophecies.Dtos;

namespace RccgHopeHouse.Application.Features.Prophecies.Commands;

/// <summary>
/// Command used to create an individual prophecy statement.
/// </summary>
public record CreateProphecyCommand(
    Guid CategoryId,
    string Text,
    int DisplayOrder)
    : IRequest<ProphecyDto>;