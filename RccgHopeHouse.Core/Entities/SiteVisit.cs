namespace RccgHopeHouse.Core.Entities;

/// <summary>
/// One anonymous visitor to the public website on one day.
/// Feeds the "Website Overview" panel on the admin dashboard.
///
/// Nothing here identifies a person. <see cref="VisitorHash"/> is a
/// one-way code made from the visitor's network address and browser
/// together with the date, so it changes every day and cannot be
/// turned back into an address. No cookie is stored on the visitor's
/// device.
///
/// This is deliberately not a <see cref="BaseEntity"/>: rows are
/// never edited by people, so they need no soft-delete or audit
/// fields.
/// </summary>
public class SiteVisit
{
    public const int VisitorHashLength = 64;

    /// <summary>The most reading time accepted in one report (30 minutes).</summary>
    public const int MaxSecondsPerReport = 30 * 60;

    /// <summary>The most reading time counted for one visitor in a day (4 hours).</summary>
    public const int MaxSecondsPerDay = 4 * 60 * 60;

    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>The day of the visit, in UK time.</summary>
    public DateOnly VisitDate { get; private set; }

    public string VisitorHash { get; private set; } = string.Empty;

    public bool IsMobile { get; private set; }

    /// <summary>How many pages this visitor opened on this day.</summary>
    public int PageViews { get; private set; }

    /// <summary>Seconds the site was open and on screen.</summary>
    public int EngagedSeconds { get; private set; }

    public DateTime FirstSeenAt { get; private set; }

    public DateTime LastSeenAt { get; private set; }

    private SiteVisit()
    {
    }

    public static SiteVisit Start(
        DateOnly visitDate,
        string visitorHash,
        bool isMobile,
        DateTime nowUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(visitorHash, nameof(visitorHash));

        if (visitorHash.Length > VisitorHashLength)
        {
            throw new ArgumentException(
                "The visitor code is too long.",
                nameof(visitorHash));
        }

        return new SiteVisit
        {
            VisitDate = visitDate,
            VisitorHash = visitorHash,
            IsMobile = isMobile,
            PageViews = 1,
            EngagedSeconds = 0,
            FirstSeenAt = nowUtc,
            LastSeenAt = nowUtc
        };
    }
}
