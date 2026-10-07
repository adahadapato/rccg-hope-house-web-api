using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

/// <summary>
/// Handles adding a contact method to the church profile.
/// </summary>
public class AddChurchContactMethodCommandHandler
    : IRequestHandler<
        AddChurchContactMethodCommand,
        ChurchInfoDto>
{
    private readonly IChurchInfoRepository
        _churchInfoRepository;

    private readonly IMemberRepository
        _memberRepository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="AddChurchContactMethodCommandHandler"/> class.
    /// </summary>
    /// <param name="churchInfoRepository">
    /// Repository used to access church information.
    /// </param>
    /// <param name="memberRepository">
    /// Repository used to retrieve member information.
    /// </param>
    public AddChurchContactMethodCommandHandler(
        IChurchInfoRepository churchInfoRepository,
        IMemberRepository memberRepository)
    {
        _churchInfoRepository =
            churchInfoRepository;

        _memberRepository =
            memberRepository;
    }

    /// <summary>
    /// Adds a new contact method to the church profile.
    /// </summary>
    /// <param name="request">
    /// The contact method details.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The updated church information.
    /// </returns>
    public async Task<ChurchInfoDto> Handle(
        AddChurchContactMethodCommand request,
        CancellationToken ct)
    {
        var info =
            await _churchInfoRepository.GetAsync(ct)
            ?? throw new NotFoundException(
                nameof(Core.Entities.ChurchInfo),
                Guid.Empty);

        var contactMethod =
            ChurchContactMethod.Create(
                info.Id,
                request.Type,
                request.Value,
                request.Label,
                request.DisplayOrder);

        info.AddContactMethod(contactMethod);

        // Explicitly register the new child entity as Added.
        // This guarantees EF Core generates an INSERT for it.
        await _churchInfoRepository
            .AddContactMethodAsync(
                contactMethod,
                ct);

        await _churchInfoRepository
            .SaveChangesAsync(ct);

        var activeMemberCount =
            await _memberRepository
                .GetActiveCountAsync(ct);

        return ChurchInfoDto.FromEntity(
            info,
            activeMemberCount);
    }
}