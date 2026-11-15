using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Exceptions;

namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// A News &amp; Updates item. Published announcements that are
/// within their display window appear in the public floating
/// announcement banner and its detail view.
/// </summary>
public class Announcement : BaseEntity
{
    public const int TitleMaxLength = 150;
    public const int SummaryMaxLength = 300;
    public const int BodyMaxLength = 4000;
    public const int LinkUrlMaxLength = 1000;
    public const int LinkTextMaxLength = 60;
    public const int ImagePathMaxLength = 500;
    public const int VenueNameMaxLength = 150;
    public const int VenueAddressMaxLength = 300;

    /// <summary>Headline shown in the banner.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Short text shown in the banner (one or two lines).</summary>
    public string? Summary { get; private set; }

    /// <summary>Full text shown when the visitor opens the announcement.</summary>
    public string? Body { get; private set; }

    public AnnouncementType Type { get; private set; } = AnnouncementType.News;

    /// <summary>Public path of the uploaded poster/flyer image.</summary>
    public string? ImagePath { get; private set; }

    /// <summary>Public path of the poster thumbnail used inside the banner.</summary>
    public string? ThumbnailPath { get; private set; }

    /// <summary>Optional call-to-action link (registration, livestream, etc.).</summary>
    public string? LinkUrl { get; private set; }

    /// <summary>Text for the call-to-action button.</summary>
    public string? LinkText { get; private set; }

    /// <summary>Optional date and time the event itself starts (UTC).</summary>
    public DateTime? EventStartsAt { get; private set; }

    /// <summary>Optional date and time the event itself ends (UTC).</summary>
    public DateTime? EventEndsAt { get; private set; }

    /// <summary>Optional venue name, e.g. "RCCG Hope House".</summary>
    public string? VenueName { get; private set; }

    /// <summary>
    /// Optional full address including postcode. Used for the
    /// map and the Google Maps directions link.
    /// </summary>
    public string? VenueAddress { get; private set; }

    /// <summary>When the announcement starts being shown (UTC).</summary>
    public DateTime StartsAt { get; private set; }

    /// <summary>When the announcement stops being shown (UTC). Null = until unpublished.</summary>
    public DateTime? EndsAt { get; private set; }

    /// <summary>Whether the announcement is live on the website.</summary>
    public bool IsPublished { get; private set; } = true;

    /// <summary>Whether the announcement appears in the floating banner.</summary>
    public bool ShowInBanner { get; private set; } = true;

    /// <summary>Lower numbers are shown first.</summary>
    public int DisplayOrder { get; private set; }

    private Announcement()
    {
    }

    public static Announcement Create(
        string title,
        AnnouncementType type,
        DateTime startsAt,
        DateTime? endsAt = null,
        string? summary = null,
        string? body = null,
        string? linkUrl = null,
        string? linkText = null,
        int displayOrder = 0,
        bool isPublished = true,
        bool showInBanner = true,
        DateTime? eventStartsAt = null,
        DateTime? eventEndsAt = null,
        string? venueName = null,
        string? venueAddress = null)
    {
        var announcement = new Announcement();

        announcement.Apply(
            title, type, startsAt, endsAt, summary, body,
            linkUrl, linkText, displayOrder, isPublished, showInBanner,
            eventStartsAt, eventEndsAt, venueName, venueAddress);

        return announcement;
    }

    public void Update(
        string title,
        AnnouncementType type,
        DateTime startsAt,
        DateTime? endsAt,
        string? summary,
        string? body,
        string? linkUrl,
        string? linkText,
        int displayOrder,
        bool isPublished,
        bool showInBanner,
        DateTime? eventStartsAt,
        DateTime? eventEndsAt,
        string? venueName,
        string? venueAddress)
    {
        Apply(
            title, type, startsAt, endsAt, summary, body,
            linkUrl, linkText, displayOrder, isPublished, showInBanner,
            eventStartsAt, eventEndsAt, venueName, venueAddress);

        MarkAsUpdated();
    }

    public void SetImage(string imagePath, string? thumbnailPath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            throw new DomainException("Image path is required.");
        }

        ImagePath = imagePath.Trim();
        ThumbnailPath = NormalizeOptionalText(thumbnailPath);

        MarkAsUpdated();
    }

    public void ClearImage()
    {
        ImagePath = null;
        ThumbnailPath = null;

        MarkAsUpdated();
    }

    public void TogglePublished()
    {
        IsPublished = !IsPublished;

        MarkAsUpdated();
    }

    /// <summary>
    /// True when the announcement should currently be visible to visitors.
    /// </summary>
    public bool IsLiveAt(DateTime utcNow) =>
        IsPublished &&
        StartsAt <= utcNow &&
        (!EndsAt.HasValue || EndsAt.Value >= utcNow);

    private void Apply(
        string title,
        AnnouncementType type,
        DateTime startsAt,
        DateTime? endsAt,
        string? summary,
        string? body,
        string? linkUrl,
        string? linkText,
        int displayOrder,
        bool isPublished,
        bool showInBanner,
        DateTime? eventStartsAt,
        DateTime? eventEndsAt,
        string? venueName,
        string? venueAddress)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Announcement title is required.");
        }

        if (endsAt.HasValue && endsAt.Value < startsAt)
        {
            throw new DomainException(
                "The announcement end date cannot be earlier than its start date.");
        }

        if (eventEndsAt.HasValue && !eventStartsAt.HasValue)
        {
            throw new DomainException(
                "An event end time needs an event start time.");
        }

        if (eventStartsAt.HasValue && eventEndsAt.HasValue &&
            eventEndsAt.Value < eventStartsAt.Value)
        {
            throw new DomainException(
                "The event cannot end before it starts.");
        }

        if (displayOrder < 0)
        {
            throw new DomainException("Display order cannot be negative.");
        }

        Title = title.Trim();
        Type = type;
        StartsAt = startsAt;
        EndsAt = endsAt;
        Summary = NormalizeOptionalText(summary);
        Body = NormalizeOptionalText(body);
        LinkUrl = NormalizeOptionalText(linkUrl);
        LinkText = LinkUrl is null ? null : NormalizeOptionalText(linkText) ?? "Learn more";
        DisplayOrder = displayOrder;
        IsPublished = isPublished;
        ShowInBanner = showInBanner;
        EventStartsAt = eventStartsAt;
        EventEndsAt = eventEndsAt;
        VenueName = NormalizeOptionalText(venueName);
        VenueAddress = NormalizeOptionalText(venueAddress);
    }

    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
