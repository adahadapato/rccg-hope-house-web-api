using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Members.Dtos;

/// <summary>
/// Represents member information returned by the application layer.
/// </summary>
public record MemberDto(
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
    string? PhotoImagePath,
    string? PhotoThumbnailPath,
    bool IsActive,
    DateTime JoinedDate)
{
    /// <summary>
    /// Creates a member DTO from the supplied member entity.
    /// </summary>
    /// <param name="member">
    /// The member entity to map.
    /// </param>
    /// <returns>
    /// A DTO containing the member's information.
    /// </returns>
    public static MemberDto FromEntity(
        Member member) => new(
        Id: member.Id,
        FirstName: member.FirstName,
        LastName: member.LastName,
        Email: member.Email?.Value,
        PhoneNumber: member.PhoneNumber?.Value,
        AddressLine1:
            member.Address?.AddressLine1,
        AddressLine2:
            member.Address?.AddressLine2,
        City:
            member.Address?.City,
        County:
            member.Address?.County,
        Postcode:
            member.Address?.Postcode,
        Country:
            member.Address?.Country,
        BirthMonth:
            member.Birthday?.Month,
        BirthDay:
            member.Birthday?.Day,
        BirthYear:
            member.Birthday?.Year,
        MaritalStatus:
            member.MaritalStatus,
        WeddingAnniversary:
            member.WeddingAnniversary,
        ConsentToContact:
            member.ConsentToContact,
        ConsentToBirthdayPublication:
            member.ConsentToBirthdayPublication,
        PhotoId:
            member.PhotoId,
        PhotoImagePath:
            member.Photo?.ImagePath,
        PhotoThumbnailPath:
            member.Photo?.ThumbnailPath,
        IsActive:
            member.IsActive,
        JoinedDate:
            member.JoinedDate);
}