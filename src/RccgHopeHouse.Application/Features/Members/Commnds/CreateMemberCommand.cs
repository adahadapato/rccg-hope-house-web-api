using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Members.Commands;

public record CreateMemberCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? PhoneNumber,
    int? BirthMonth,
    int? BirthDay,
    int? BirthYear,
    MaritalStatus? MaritalStatus,
    DateOnly? WeddingAnniversary,
    bool ConsentToContact) : IRequest<MemberDto>;