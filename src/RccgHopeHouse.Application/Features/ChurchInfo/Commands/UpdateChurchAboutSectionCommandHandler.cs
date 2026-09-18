using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

public class UpdateChurchAboutSectionCommandHandler : IRequestHandler<UpdateChurchAboutSectionCommand, ChurchInfoDto>
{
    private readonly IChurchInfoRepository _churchInfoRepository;
    private readonly IMemberRepository _memberRepository;

    public UpdateChurchAboutSectionCommandHandler(
        IChurchInfoRepository churchInfoRepository,
        IMemberRepository memberRepository)
    {
        _churchInfoRepository = churchInfoRepository;
        _memberRepository = memberRepository;
    }

    public async Task<ChurchInfoDto> Handle(UpdateChurchAboutSectionCommand request, CancellationToken ct)
    {
        var info = await _churchInfoRepository.GetAsync(ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ChurchInfo), Guid.Empty);

        info.UpdateAboutSection(
            request.ParishName, request.EstablishedYear, request.Tagline,
            request.AboutLead, request.AboutText, request.MultiCulturalStat);

        await _churchInfoRepository.UpdateAsync(info, ct);
        await _churchInfoRepository.SaveChangesAsync(ct);

        var activeMemberCount = await _memberRepository.GetActiveCountAsync(ct);
        return ChurchInfoDto.FromEntity(info, activeMemberCount);
    }
}