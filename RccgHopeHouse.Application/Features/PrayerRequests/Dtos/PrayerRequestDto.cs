using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Dtos;

/// <summary>
/// Represents a prayer request returned by the application layer.
/// </summary>
/// <remarks>
/// Contains the requester's available contact information,
/// prayer request content, pastoral workflow status, pastoral note,
/// and response information required by the administration interface.
/// </remarks>
public record PrayerRequestDto(
    Guid Id,
    string RequesterName,
    bool IsAnonymous,
    string? RequesterEmail,
    string? PhoneNumber,
    string Content,
    PrayerRequestStatus Status,
    DateTime CreatedAt,
    string? PastoralNote,
    DateTime? RespondedAt)
{
    /// <summary>
    /// Creates a <see cref="PrayerRequestDto"/> from a
    /// <see cref="PrayerRequest"/> domain entity.
    /// </summary>
    /// <param name="request">
    /// The prayer request entity to map.
    /// </param>
    /// <returns>
    /// A DTO containing the prayer request information.
    /// </returns>
    public static PrayerRequestDto FromEntity(
        PrayerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new PrayerRequestDto(
            Id: request.Id,
            RequesterName:
                request.RequesterName,
            IsAnonymous:
                request.IsAnonymous,
            RequesterEmail:
                request.RequesterEmail?.Value,
            PhoneNumber:
                request.PhoneNumber?.Value,
            Content:
                request.Content,
            Status:
                request.Status,
            CreatedAt:
                request.CreatedAt,
            PastoralNote:
                request.PastoralNote,
            RespondedAt:
                request.RespondedAt);
    }
}