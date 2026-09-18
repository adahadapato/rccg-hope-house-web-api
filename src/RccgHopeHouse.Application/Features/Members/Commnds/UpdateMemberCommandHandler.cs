using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _repository;

    public UpdateMemberCommandHandler(IMemberRepository repository) => _repository = repository;

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        member.UpdateProfile(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
        member.UpdateBirthday(request.BirthMonth, request.BirthDay, request.BirthYear);
        member.UpdateMaritalInfo(request.MaritalStatus, request.WeddingAnniversary);
        member.SetConsentToContact(request.ConsentToContact);

        await _repository.UpdateAsync(member, ct);
        await _repository.SaveChangesAsync(ct);

        return MemberDto.FromEntity(member);
    }
}