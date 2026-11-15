using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a prayer request submitted through RCCG Hope House.
/// </summary>
public class PrayerRequest : BaseEntity
{
    /// <summary>
    /// Gets the name of the person who submitted the prayer request.
    /// Anonymous requests use "Anonymous".
    /// </summary>
    public string RequesterName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the prayer request was
    /// submitted anonymously.
    /// </summary>
    public bool IsAnonymous { get; private set; }

    /// <summary>
    /// Gets the requester's email address when one was supplied.
    /// Anonymous requests do not retain an email address.
    /// </summary>
    public EmailAddress? RequesterEmail { get; private set; }

    /// <summary>
    /// Gets the prayer request content.
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the requester's telephone number when one was supplied.
    /// Anonymous requests do not retain a telephone number.
    /// </summary>
    public PhoneNumber? PhoneNumber { get; private set; }

    /// <summary>
    /// Gets the latest internal pastoral note associated with
    /// the prayer request.
    /// </summary>
    public string? PastoralNote { get; private set; }

    /// <summary>
    /// Gets the current pastoral workflow status.
    /// </summary>
    public PrayerRequestStatus Status { get; private set; } =
        PrayerRequestStatus.Pending;

    /// <summary>
    /// Gets the UTC date and time at which the prayer request
    /// was most recently marked as resolved.
    /// </summary>
    public DateTime? RespondedAt { get; private set; }

    /// <summary>
    /// Parameterless constructor required by Entity Framework Core.
    /// </summary>
    private PrayerRequest()
    {
    }

    /// <summary>
    /// Creates a new prayer request.
    /// </summary>
    /// <param name="content">
    /// The prayer request submitted by the requester.
    /// </param>
    /// <param name="isAnonymous">
    /// Indicates whether the request should be anonymous.
    /// </param>
    /// <param name="requesterName">
    /// The requester's name when the request is not anonymous.
    /// </param>
    /// <param name="phoneNumber">
    /// Optional requester telephone number.
    /// </param>
    /// <param name="requesterEmail">
    /// Optional requester email address.
    /// </param>
    /// <returns>
    /// A new <see cref="PrayerRequest"/> instance.
    /// </returns>
    public static PrayerRequest Create(
        string content,
        bool isAnonymous,
        string? requesterName = null,
        string? phoneNumber = null,
        string? requesterEmail = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            content,
            nameof(content));

        if (!isAnonymous)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                requesterName,
                nameof(requesterName));
        }

        return new PrayerRequest
        {
            RequesterName = isAnonymous
                ? "Anonymous"
                : requesterName!.Trim(),

            IsAnonymous = isAnonymous,

            Content = content.Trim(),

            RequesterEmail = isAnonymous
                ? null
                : EmailAddress.CreateOrNull(
                    requesterEmail),

            PhoneNumber = isAnonymous
                ? null
                : PhoneNumber.CreateOrNull(
                    phoneNumber)
        };
    }

    /// <summary>
    /// Marks the prayer request as resolved while preserving the
    /// existing pastoral note.
    /// </summary>
    public void MarkAsResolved()
    {
        UpdateStatus(
            PrayerRequestStatus.Resolved,
            PastoralNote);
    }

    /// <summary>
    /// Updates the pastoral workflow status and pastoral note.
    ///
    /// When the request first moves to Resolved, the resolved
    /// timestamp is recorded. If a resolved request is reopened,
    /// the timestamp is cleared because it is no longer resolved.
    /// </summary>
    /// <param name="newStatus">
    /// The new pastoral workflow status.
    /// </param>
    /// <param name="pastoralNote">
    /// The optional pastoral note to associate with the request.
    /// </param>
    public void UpdateStatus(
        PrayerRequestStatus newStatus,
        string? pastoralNote)
    {
        if (
            newStatus == PrayerRequestStatus.Resolved &&
            Status != PrayerRequestStatus.Resolved)
        {
            RespondedAt = DateTime.UtcNow;
        }
        else if (
            newStatus != PrayerRequestStatus.Resolved)
        {
            RespondedAt = null;
        }

        Status = newStatus;

        PastoralNote =
            string.IsNullOrWhiteSpace(
                pastoralNote)
                ? null
                : pastoralNote.Trim();

        MarkAsUpdated();
    }
}