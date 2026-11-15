using MediatR;
using RccgHopeHouse.Application.Features.Members.Dtos;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Members.Commands;

/// <summary>
/// Updates an existing church member.
/// </summary>
public record UpdateMemberCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? PhoneNumber,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? County,
    string? Postcode,
    string? Country,
    int? BirthMonth,
    int? BirthDay,
    int? BirthYear,
    MaritalStatus? MaritalStatus,
    DateOnly? WeddingAnniversary,
    bool ConsentToContact,
    bool ConsentToBirthdayPublication,
    Guid? PhotoId,
    DateTime? JoinedDate) : IRequest<MemberDto>;