using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _repository;

    public CreateMemberCommandHandler(IMemberRepository repository) => _repository = repository;

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken ct)
    {
        var member = Member.Create(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phoneNumber: request.PhoneNumber,
            birthMonth: request.BirthMonth,
            birthDay: request.BirthDay,
            birthYear: request.BirthYear,
            maritalStatus: request.MaritalStatus,
            weddingAnniversary: request.WeddingAnniversary,
            consentToContact: request.ConsentToContact);

        await _repository.AddAsync(member, ct);
        await _repository.SaveChangesAsync(ct);

        return MemberDto.FromEntity(member);
    }
}