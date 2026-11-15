using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Handles updates to an existing church contact method.
/// </summary>
public class UpdateChurchContactMethodCommandHandler
    : IRequestHandler<UpdateChurchContactMethodCommand, ChurchInfoDto>
{
    private readonly IChurchInfoRepository _churchInfoRepository;
    private readonly IMemberRepository _memberRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UpdateChurchContactMethodCommandHandler"/> class.
    /// </summary>
    public UpdateChurchContactMethodCommandHandler(
        IChurchInfoRepository churchInfoRepository,
        IMemberRepository memberRepository)
    {
        _churchInfoRepository = churchInfoRepository;
        _memberRepository = memberRepository;
    }

    /// <summary>
    /// Updates the requested contact method.
    /// </summary>
    public async Task<ChurchInfoDto> Handle(
        UpdateChurchContactMethodCommand request,
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

        contactMethod.Update(
            request.Type,
            request.Value,
            request.Label,
            request.DisplayOrder);

        await _churchInfoRepository.UpdateAsync(info, ct);
        await _churchInfoRepository.SaveChangesAsync(ct);

        var activeMemberCount =
            await _memberRepository.GetActiveCountAsync(ct);

        return ChurchInfoDto.FromEntity(
            info,
            activeMemberCount);
    }
}