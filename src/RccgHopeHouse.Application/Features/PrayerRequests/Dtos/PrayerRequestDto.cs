using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Dtos;

/// <summary>
/// Data Transfer Object for prayer requests.
/// Used for admin management and detailed public views (if approved).
/// </summary>
public record PrayerRequestDto(
    Guid Id,
    string RequesterName,
    bool IsAnonymous,
    string? RequesterEmail,
    string? PhoneNumber, // Serialized as string
    string Content,
    PrayerRequestStatus Status,
    DateTime CreatedAt,
    string? PastoralNote,
    DateTime? RespondedAt)
{
    public static PrayerRequestDto FromEntity(PrayerRequest request) => new(
        Id: request.Id,
        RequesterName: request.RequesterName,
        IsAnonymous: request.IsAnonymous,
        RequesterEmail: request.RequesterEmail?.Value,
        PhoneNumber: request.PhoneNumber?.Value,
        Content: request.Content,
        Status: request.Status,
        CreatedAt: request.CreatedAt,
        PastoralNote: request.PastoralNote,
        RespondedAt: request.RespondedAt);
}