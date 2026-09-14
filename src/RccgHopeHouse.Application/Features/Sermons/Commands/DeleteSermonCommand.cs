using MediatR;

namespace RccgHopeHouse.Application.Features.Sermons.Commands;

public record DeleteSermonCommand(Guid Id) : IRequest<Unit>;