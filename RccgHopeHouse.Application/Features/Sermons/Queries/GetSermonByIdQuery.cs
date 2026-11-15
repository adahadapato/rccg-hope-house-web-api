using MediatR;
using RccgHopeHouse.Application.Features.Sermons.Dtos;

namespace RccgHopeHouse.Application.Features.Sermons.Queries;

public record GetSermonByIdQuery(Guid Id) : IRequest<SermonDto>;