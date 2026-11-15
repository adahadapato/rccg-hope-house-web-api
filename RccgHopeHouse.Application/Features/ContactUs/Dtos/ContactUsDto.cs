using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ContactUs.Dtos;

/// <summary>
/// DTO for contact request admin views.
/// Excludes sensitive data; includes metadata for inbox management.
/// </summary>
public record ContactUsDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    ContactReason Reason,
    string Message,
    bool IsRead,
    DateTime? RespondedAt,
    DateTime CreatedAt)
{
    public static ContactUsDto FromEntity(Core.Entities.ContactUs contact) => new(
        Id: contact.Id,
        FirstName: contact.FirstName,
        LastName: contact.LastName,
        Email: contact.Email.Value,
        PhoneNumber: contact.PhoneNumber?.Value,
        Reason: contact.Reason,
        Message: contact.Message,
        IsRead: contact.IsRead,
        RespondedAt: contact.RespondedAt,
        CreatedAt: contact.CreatedAt);
}