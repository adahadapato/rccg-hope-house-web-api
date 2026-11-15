using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.ValueObjects;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Someone who worshipped with the church for the first time and
/// filled in the welcome form (opened from the QR code).
///
/// First-timers are kept apart from <see cref="Member"/> so that
/// one-off visitors never appear in the members list or receive
/// birthday greetings. An admin promotes a first-timer to a member
/// once they have settled in.
///
/// The prayer request is NOT stored here. It is saved as a normal
/// <see cref="PrayerRequest"/> and linked by <see cref="PrayerRequestId"/>,
/// so only the prayer team can read it.
/// </summary>
public class FirstTimer : BaseEntity
{
    public const int NameMaxLength = 100;
    public const int CommentMaxLength = 1000;
    public const int FollowUpNoteMaxLength = 1000;
    public const int InvitedByMaxLength = 150;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    /// <summary>The phone number exactly as the visitor typed it.</summary>
    public string PhoneNumber { get; private set; } = string.Empty;

    /// <summary>
    /// The phone number in one standard form, used to spot the same
    /// visitor filling in the form again.
    /// </summary>
    public string NormalizedPhoneNumber { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? AddressLine1 { get; private set; }

    public string? AddressLine2 { get; private set; }

    public string? City { get; private set; }

    public string? Postcode { get; private set; }

    public int? BirthMonth { get; private set; }

    public int? BirthDay { get; private set; }

    public FirstTimerSource? Source { get; private set; }

    public string? InvitedBy { get; private set; }

    public bool WantsPastoralContact { get; private set; }

    public FirstTimerContactMethod PreferredContactMethod { get; private set; }

    public string? Comment { get; private set; }

    public bool ConsentToContact { get; private set; }

    public DateOnly FirstVisitDate { get; private set; }

    public DateOnly LastVisitDate { get; private set; }

    public int VisitCount { get; private set; } = 1;

    public FirstTimerFollowUpStatus FollowUpStatus { get; private set; } =
        FirstTimerFollowUpStatus.NotContacted;

    public string? FollowUpNote { get; private set; }

    /// <summary>The linked confidential prayer request, if one was made.</summary>
    public Guid? PrayerRequestId { get; private set; }

    /// <summary>The member record created when this visitor was promoted.</summary>
    public Guid? MemberId { get; private set; }

    public DateTime? PromotedAt { get; private set; }

    private FirstTimer()
    {
    }

    public static FirstTimer Create(
        string firstName,
        string lastName,
        string phoneNumber,
        bool consentToContact,
        FirstTimerContactMethod preferredContactMethod,
        DateOnly visitDate,
        string? email = null,
        string? addressLine1 = null,
        string? addressLine2 = null,
        string? city = null,
        string? postcode = null,
        int? birthMonth = null,
        int? birthDay = null,
        FirstTimerSource? source = null,
        string? invitedBy = null,
        bool wantsPastoralContact = false,
        string? comment = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

        if (!consentToContact)
        {
            throw new ArgumentException(
                "Consent is required before a visitor's details can be stored.",
                nameof(consentToContact));
        }

        // Validates the format; throws ArgumentException if it is wrong.
        var phone = ValueObjects.PhoneNumber.Create(phoneNumber);
        var emailAddress = EmailAddress.CreateOrNull(email);

        if (birthMonth.HasValue != birthDay.HasValue)
        {
            throw new ArgumentException(
                "Birth month and day must be given together.");
        }

        if (birthMonth.HasValue && birthDay.HasValue)
        {
            // Throws if the day does not exist in that month.
            Birthday.Create(birthMonth.Value, birthDay.Value);
        }

        return new FirstTimer
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            PhoneNumber = phone.Value,
            NormalizedPhoneNumber = NormalizePhone(phone.Value),
            Email = emailAddress?.Value,
            AddressLine1 = Clean(addressLine1),
            AddressLine2 = Clean(addressLine2),
            City = Clean(city),
            Postcode = Clean(postcode),
            BirthMonth = birthMonth,
            BirthDay = birthDay,
            Source = source,
            InvitedBy = Clean(invitedBy),
            WantsPastoralContact = wantsPastoralContact,
            PreferredContactMethod = preferredContactMethod,
            Comment = Clean(comment),
            ConsentToContact = true,
            FirstVisitDate = visitDate,
            LastVisitDate = visitDate,
            VisitCount = 1
        };
    }

    /// <summary>
    /// Puts a phone number into one standard form so that
    /// "07123 456789" and "+447123456789" are seen as the same number.
    /// </summary>
    public static string NormalizePhone(string phoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

        try
        {
            return ValueObjects.PhoneNumber.Create(phoneNumber).ToE164();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return new string(phoneNumber.Where(char.IsAsciiDigit).ToArray());
        }
    }

    /// <summary>
    /// Called when the same phone number submits the form again.
    /// The name, address and contact details already on record are
    /// deliberately left alone: the form is public, so a second
    /// submission must not be able to overwrite someone's details.
    /// </summary>
    public void RecordReturnVisit(DateOnly visitDate, string? comment)
    {
        if (visitDate > LastVisitDate)
        {
            LastVisitDate = visitDate;
            VisitCount++;
        }

        var extra = Clean(comment);

        if (extra is not null)
        {
            var combined = Comment is null
                ? extra
                : $"{Comment}\n{extra}";

            Comment = combined.Length > CommentMaxLength
                ? combined[..CommentMaxLength]
                : combined;
        }

        MarkAsUpdated();
    }

    public void LinkPrayerRequest(Guid prayerRequestId)
    {
        if (prayerRequestId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid prayer request ID is required.",
                nameof(prayerRequestId));
        }

        PrayerRequestId = prayerRequestId;
        MarkAsUpdated();
    }

    public void UpdateFollowUp(FirstTimerFollowUpStatus status, string? note)
    {
        FollowUpStatus = status;
        FollowUpNote = Clean(note);
        MarkAsUpdated();
    }

    public void MarkPromoted(Guid memberId)
    {
        if (memberId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid member ID is required.",
                nameof(memberId));
        }

        MemberId = memberId;
        PromotedAt = DateTime.UtcNow;
        FollowUpStatus = FirstTimerFollowUpStatus.Closed;
        MarkAsUpdated();
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
