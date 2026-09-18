using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Members.Queries;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto>
{
    private readonly IMemberRepository _repository;

    public GetMemberByIdQueryHandler(IMemberRepository repository) => _repository = repository;

    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        return MemberDto.FromEntity(member);
    }
}