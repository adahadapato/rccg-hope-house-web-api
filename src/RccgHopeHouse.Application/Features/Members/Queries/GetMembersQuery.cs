using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;

namespace RccgHopeHouse.Application.Features.Members.Queries;

public record GetMembersQuery(bool IncludeInactive, int Skip, int Take) : IRequest<IReadOnlyList<MemberDto>>;