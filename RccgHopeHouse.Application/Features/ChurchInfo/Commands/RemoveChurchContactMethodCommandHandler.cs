using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Handles removal of a church contact method.
/// </summary>
public class RemoveChurchContactMethodCommandHandler
    : IRequestHandler<RemoveChurchContactMethodCommand, ChurchInfoDto>
{
    private readonly IChurchInfoRepository _churchInfoRepository;
    private readonly IMemberRepository _memberRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="RemoveChurchContactMethodCommandHandler"/> class.
    /// </summary>
    public RemoveChurchContactMethodCommandHandler(
        IChurchInfoRepository churchInfoRepository,
        IMemberRepository memberRepository)
    {
        _churchInfoRepository = churchInfoRepository;
        _memberRepository = memberRepository;
    }

    /// <summary>
    /// Removes the requested contact method from the church profile.
    /// </summary>
    public async Task<ChurchInfoDto> Handle(
        RemoveChurchContactMethodCommand request,
        CancellationToken ct)
    {
        var info = await _churchInfoRepository.GetAsync(ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchInfo),
                Guid.Empty);

        var contactMethod = info.ContactMethods
            .FirstOrDefault(method => method.Id == request.Id)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchContactMethod),
                request.Id);

        info.RemoveContactMethod(contactMethod.Id);

        await _churchInfoRepository.UpdateAsync(info, ct);
        await _churchInfoRepository.SaveChangesAsync(ct);

        var activeMemberCount =
            await _memberRepository.GetActiveCountAsync(ct);

        return ChurchInfoDto.FromEntity(
            info,
            activeMemberCount);
    }
}