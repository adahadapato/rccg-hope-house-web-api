
namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// Represents a member's birthday or wedding anniversary celebration,
/// including invitation, consent, approval and publication information.
/// </summary>
public class Celebration : BaseEntity
{
    /// <summary>
    /// Gets the member associated with this celebration.
    /// </summary>
    public Guid MemberId { get; private set; }

    /// <summary>
    /// Gets the member navigation property.
    /// </summary>
    public Member Member { get; private set; } = null!;

    /// <summary>
    /// Gets the gallery category associated with this celebration.
    /// </summary>
    public Guid GalleryCategoryId { get; private set; }

    /// <summary>
    /// Gets the gallery category navigation property.
    /// </summary>
    public GalleryCategory GalleryCategory { get; private set; } = null!;

    /// <summary>
    /// Gets the type of celebration: Birthday or WeddingAnniversary.
    /// </summary>
    public string Occasion { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the scheduled celebration date.
    /// </summary>
    public DateOnly CelebrationDate { get; private set; }

    /// <summary>
    /// Gets the identifier of the gallery image selected for this celebration.
    /// A null value indicates that no gallery photograph has been selected.
    /// </summary>
    public Guid? GalleryImageId { get; private set; }

    /// <summary>
    /// Gets the gallery image selected for this celebration.
    /// </summary>
    public GalleryImage? GalleryImage { get; private set; }

    /// <summary>
    /// Gets the legacy photograph URL, retained for compatibility.
    /// </summary>
    public string? PhotoUrl { get; private set; }

    /// <summary>
    /// Indicates whether the member has explicitly consented to
    /// publication of the selected celebration photograph.
    /// </summary>
    public bool PhotoPublicationConsent { get; private set; }

    /// <summary>
    /// Gets the current celebration workflow status.
    /// </summary>
    public string Status { get; private set; } = "Pending";

    /// <summary>
    /// Gets the UTC time the invitation was sent.
    /// </summary>
    public DateTime? InvitationSentAt { get; private set; }

    /// <summary>
    /// Gets the UTC time the member responded.
    /// </summary>
    public DateTime? RespondedAt { get; private set; }

    /// <summary>
    /// Gets the UTC time an administrator approved publication.
    /// </summary>
    public DateTime? ApprovedByAdminAt { get; private set; }

    /// <summary>
    /// Gets the UTC time the celebration was published.
    /// </summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>
    /// Gets the UTC time publication consent was withdrawn.
    /// </summary>
    public DateTime? WithdrawnAt { get; private set; }

    /// <summary>
    /// Required by Entity Framework Core.
    /// </summary>
    private Celebration()
    {
    }

    /// <summary>
    /// Creates a new pending celebration.
    /// </summary>
    public static Celebration Create(
        Guid memberId,
        Guid galleryCategoryId,
        string occasion,
        DateOnly celebrationDate)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("A valid member ID is required.", nameof(memberId));

        if (galleryCategoryId == Guid.Empty)
            throw new ArgumentException("A valid gallery category ID is required.", nameof(galleryCategoryId));

        if (occasion is not ("Birthday" or "WeddingAnniversary"))
            throw new ArgumentException("Invalid celebration occasion.", nameof(occasion));

        return new Celebration
        {
            MemberId = memberId,
            GalleryCategoryId = galleryCategoryId,
            Occasion = occasion,
            CelebrationDate = celebrationDate,
            Status = "Pending"
        };
    }

    /// <summary>
    /// Records that the invitation was successfully sent.
    /// </summary>
    public void MarkInvitationSent(DateTime sentAtUtc)
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending celebrations can receive invitations.");

        if (InvitationSentAt.HasValue)
            throw new InvalidOperationException("An invitation has already been sent.");

        InvitationSentAt = sentAtUtc;
        MarkAsUpdated();
    }

    /// <summary>
    /// Records the member's approval to publish the celebration.
    /// </summary>
    public void ApprovePublication(DateTime respondedAtUtc)
    {
        EnsureAwaitingMemberResponse();

        Status = "ApprovedByMember";
        RespondedAt = respondedAtUtc;
        PhotoPublicationConsent = false;
        PhotoUrl = null;
        GalleryImageId = null;
        GalleryImage = null;

        MarkAsUpdated();
    }

    /// <summary>
    /// Records the member's decision not to publish the celebration.
    /// </summary>
    public void Decline(DateTime respondedAtUtc)
    {
        EnsureAwaitingMemberResponse();

        Status = "Declined";
        RespondedAt = respondedAtUtc;
        ClearPhoto();

        MarkAsUpdated();
    }

    /// <summary>
    /// Assigns an existing gallery image as the celebration photograph.
    /// The member must already have approved celebration publication.
    /// </summary>
    public void SetGalleryImage(Guid galleryImageId, bool photoPublicationConsent)
    {
        if (Status != "ApprovedByMember")
            throw new InvalidOperationException("The member must approve celebration publication before selecting a photograph.");

        if (galleryImageId == Guid.Empty)
            throw new ArgumentException("A valid gallery image ID is required.", nameof(galleryImageId));

        if (!photoPublicationConsent)
            throw new InvalidOperationException("Explicit photo publication consent is required.");

        GalleryImageId = galleryImageId;
        GalleryImage = null;
        PhotoUrl = null;
        PhotoPublicationConsent = true;

        MarkAsUpdated();
    }

    /// <summary>
    /// Sets a legacy photograph URL.
    /// Retained for compatibility with existing celebration records.
    /// </summary>
    public void SetPhoto(string? photoUrl, bool consent)
    {
        if (Status != "ApprovedByMember")
            throw new InvalidOperationException("The member must approve celebration publication before setting a photograph.");

        if (!string.IsNullOrWhiteSpace(photoUrl) && !consent)
            throw new InvalidOperationException("Explicit photo publication consent is required.");

        PhotoUrl = string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl.Trim();
        GalleryImageId = null;
        GalleryImage = null;
        PhotoPublicationConsent = PhotoUrl is not null && consent;

        MarkAsUpdated();
    }

    /// <summary>
    /// Approves the celebration for scheduled publication.
    /// </summary>
    public void ApproveByAdmin(DateTime approvedAtUtc)
    {
        if (Status != "ApprovedByMember")
            throw new InvalidOperationException("Only member-approved celebrations can receive administrator approval.");

        Status = "ReadyToPublish";
        ApprovedByAdminAt = approvedAtUtc;

        MarkAsUpdated();
    }

    /// <summary>
    /// Marks an administrator-approved celebration as published.
    /// </summary>
    public void MarkPublished(DateTime publishedAtUtc)
    {
        if (Status != "ReadyToPublish")
            throw new InvalidOperationException("Only celebrations ready for publication can be published.");

        Status = "Published";
        PublishedAt = publishedAtUtc;

        MarkAsUpdated();
    }

    /// <summary>
    /// Withdraws publication consent and removes the selected
    /// photograph from the celebration record.
    /// </summary>
    public void Withdraw(DateTime withdrawnAtUtc)
    {
        if (Status is not ("ApprovedByMember" or "ReadyToPublish" or "Published"))
            throw new InvalidOperationException("This celebration cannot be withdrawn in its current state.");

        Status = "Withdrawn";
        WithdrawnAt = withdrawnAtUtc;
        ClearPhoto();

        MarkAsUpdated();
    }

    /// <summary>
    /// Updates a pending celebration before the member responds.
    /// </summary>
    public void UpdateDetails(
        Guid galleryCategoryId,
        string occasion,
        DateOnly celebrationDate)
    {
        if (Status != "Pending" || RespondedAt.HasValue)
            throw new InvalidOperationException("Only pending, unanswered celebrations can be updated.");

        if (galleryCategoryId == Guid.Empty)
            throw new ArgumentException("A valid gallery category ID is required.", nameof(galleryCategoryId));

        if (occasion is not ("Birthday" or "WeddingAnniversary"))
            throw new ArgumentException("Invalid celebration occasion.", nameof(occasion));

        var detailsChanged = GalleryCategoryId != galleryCategoryId ||
                             Occasion != occasion ||
                             CelebrationDate != celebrationDate;

        if (!detailsChanged)
            return;

        GalleryCategoryId = galleryCategoryId;
        Occasion = occasion;
        CelebrationDate = celebrationDate;
        GalleryCategory = null!;

        // A previously sent invitation must not authorise changed details.
        InvitationSentAt = null;

        MarkAsUpdated();
    }

    /// <summary>
    /// Ensures an invitation was sent and the member has not responded.
    /// </summary>
    private void EnsureAwaitingMemberResponse()
    {
        if (Status != "Pending" || !InvitationSentAt.HasValue || RespondedAt.HasValue)
            throw new InvalidOperationException("This celebration is not awaiting a member response.");
    }

    /// <summary>
    /// Removes photograph references and publication consent.
    /// Physical image deletion is handled separately by the application layer.
    /// </summary>
    private void ClearPhoto()
    {
        PhotoUrl = null;
        GalleryImageId = null;
        GalleryImage = null;
        PhotoPublicationConsent = false;
    }
}
