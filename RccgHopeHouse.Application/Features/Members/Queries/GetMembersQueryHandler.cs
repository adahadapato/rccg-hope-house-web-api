using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Members.Queries;

public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IReadOnlyList<MemberDto>>
{
    private readonly IMemberRepository _repository;

    public GetMembersQueryHandler(IMemberRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<MemberDto>> Handle(GetMembersQuery request, CancellationToken ct)
    {
        var members = await _repository.GetAllAsync(request.IncludeInactive, request.Skip, request.Take, ct);
        return members.Select(MemberDto.FromEntity).ToList();
    }
}