using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.FirstTimers.Dtos;

/// <summary>
/// What the public welcome form gets back after submitting.
/// </summary>
public record SubmitFirstTimerResultDto(string Message);

/// <summary>
/// A first-time visitor as shown in the admin area.
///
/// The prayer request text is never included. Only
/// <see cref="HasPrayerRequest"/> and its ID are returned; the text
/// is read through the prayer request endpoints, which are limited
/// to the prayer team.
/// </summary>
public record FirstTimerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    int? BirthMonth,
    int? BirthDay,
    FirstTimerSource? Source,
    string? InvitedBy,
    bool WantsPastoralContact,
    FirstTimerContactMethod PreferredContactMethod,
    string? Comment,
    bool ConsentToContact,
    DateOnly FirstVisitDate,
    DateOnly LastVisitDate,
    int VisitCount,
    FirstTimerFollowUpStatus FollowUpStatus,
    string? FollowUpNote,
    bool HasPrayerRequest,
    Guid? PrayerRequestId,
    Guid? MemberId,
    DateTime? PromotedAt,
    DateTime CreatedAt)
{
    public static FirstTimerDto FromEntity(FirstTimer firstTimer)
    {
        ArgumentNullException.ThrowIfNull(firstTimer);

        return new FirstTimerDto(
            Id: firstTimer.Id,
            FirstName: firstTimer.FirstName,
            LastName: firstTimer.LastName,
            PhoneNumber: firstTimer.PhoneNumber,
            Email: firstTimer.Email,
            AddressLine1: firstTimer.AddressLine1,
            AddressLine2: firstTimer.AddressLine2,
            City: firstTimer.City,
            Postcode: firstTimer.Postcode,
            BirthMonth: firstTimer.BirthMonth,
            BirthDay: firstTimer.BirthDay,
            Source: firstTimer.Source,
            InvitedBy: firstTimer.InvitedBy,
            WantsPastoralContact: firstTimer.WantsPastoralContact,
            PreferredContactMethod: firstTimer.PreferredContactMethod,
            Comment: firstTimer.Comment,
            ConsentToContact: firstTimer.ConsentToContact,
            FirstVisitDate: firstTimer.FirstVisitDate,
            LastVisitDate: firstTimer.LastVisitDate,
            VisitCount: firstTimer.VisitCount,
            FollowUpStatus: firstTimer.FollowUpStatus,
            FollowUpNote: firstTimer.FollowUpNote,
            HasPrayerRequest: firstTimer.PrayerRequestId.HasValue,
            PrayerRequestId: firstTimer.PrayerRequestId,
            MemberId: firstTimer.MemberId,
            PromotedAt: firstTimer.PromotedAt,
            CreatedAt: firstTimer.CreatedAt);
    }
}

/// <summary>
/// Counts for the admin page and the notification bell.
/// </summary>
public record FirstTimerStatsDto(
    int NotContacted,
    int Contacted,
    int Visited,
    int Closed,
    int Promoted);

/// <summary>
/// Result of "Add to members".
///
/// When <see cref="Promoted"/> is false and
/// <see cref="PossibleDuplicateMemberId"/> is set, a member with the
/// same phone number or email already exists. Ask the admin to
/// confirm, then send the request again with Force = true.
/// </summary>
public record PromoteFirstTimerResultDto(
    bool Promoted,
    Guid? MemberId,
    string Message,
    Guid? PossibleDuplicateMemberId = null,
    string? PossibleDuplicateMemberName = null);
