using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;

namespace RccgHopeHouse.Application.Features.Sermons.Commands;

public record UpdateSermonCommand(
    Guid Id, string Title, string Speaker, DateTime ServiceDate,
    string VideoUrl, string? Description) : IRequest<SermonDto>;