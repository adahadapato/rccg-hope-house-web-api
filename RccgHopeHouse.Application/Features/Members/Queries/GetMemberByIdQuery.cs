using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;

namespace RccgHopeHouse.Application.Features.Members.Queries;

public record GetMemberByIdQuery(Guid Id) : IRequest<MemberDto>;