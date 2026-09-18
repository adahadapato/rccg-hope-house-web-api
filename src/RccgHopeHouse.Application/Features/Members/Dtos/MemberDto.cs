using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Members.Dtos;

public record MemberDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? PhoneNumber,
    int? BirthMonth,
    int? BirthDay,
    int? BirthYear,
    MaritalStatus? MaritalStatus,
    DateOnly? WeddingAnniversary,
    bool ConsentToContact,
    bool IsActive,
    DateTime JoinedDate)
{
    public static MemberDto FromEntity(Member member) => new(
        Id: member.Id,
        FirstName: member.FirstName,
        LastName: member.LastName,
        Email: member.Email?.Value,
        PhoneNumber: member.PhoneNumber?.Value,
        BirthMonth: member.BirthMonth,
        BirthDay: member.BirthDay,
        BirthYear: member.BirthYear,
        MaritalStatus: member.MaritalStatus,
        WeddingAnniversary: member.WeddingAnniversary,
        ConsentToContact: member.ConsentToContact,
        IsActive: member.IsActive,
        JoinedDate: member.JoinedDate);
}