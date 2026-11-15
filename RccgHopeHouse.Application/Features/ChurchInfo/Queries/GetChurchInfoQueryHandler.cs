using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Queries;

public class GetChurchInfoQueryHandler : IRequestHandler<GetChurchInfoQuery, ChurchInfoDto>
{
    private readonly IChurchInfoRepository _churchInfoRepository;
    private readonly IMemberRepository _memberRepository;

    public GetChurchInfoQueryHandler(
        IChurchInfoRepository churchInfoRepository,
        IMemberRepository memberRepository)
    {
        _churchInfoRepository = churchInfoRepository;
        _memberRepository = memberRepository;
    }

    public async Task<ChurchInfoDto> Handle(GetChurchInfoQuery request, CancellationToken ct)
    {
        var info = await _churchInfoRepository.GetAsync(ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchInfo), Guid.Empty);

        var activeMemberCount = await _memberRepository.GetActiveCountAsync(ct);

        return ChurchInfoDto.FromEntity(info, activeMemberCount);
    }
}