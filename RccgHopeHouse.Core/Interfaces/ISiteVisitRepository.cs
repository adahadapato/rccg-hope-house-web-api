namespace RccgHopeHouse.Core.Interfaces;

/// <summary>Website visit totals for one day.</summary>
public record SiteVisitDayTotals(
    DateOnly Date,
    int Visitors,
    int PageViews,
    int EngagedSeconds,
    int MobileVisitors);

public interface ISiteVisitRepository
{
    /// <summary>
    /// Counts one page view. Creates the visitor's row for the day
    /// if this is their first page, otherwise adds to it.
    /// </summary>
    Task RecordPageViewAsync(
        DateOnly visitDate,
        string visitorHash,
        bool isMobile,
        DateTime nowUtc,
        CancellationToken ct = default);

    /// <summary>
    /// Adds reading time to a visitor's row for the day. Does nothing
    /// if the visitor has no row (no page view was counted).
    /// </summary>
    Task AddEngagedSecondsAsync(
        DateOnly visitDate,
        string visitorHash,
        int seconds,
        DateTime nowUtc,
        CancellationToken ct = default);

    /// <summary>One entry per day that had visits, oldest first.</summary>
    Task<IReadOnlyList<SiteVisitDayTotals>> GetDailyTotalsAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);

    /// <summary>Visitors seen since the given moment.</summary>
    Task<int> GetActiveCountAsync(
        DateTime sinceUtc,
        CancellationToken ct = default);

    /// <summary>The first day with any visit, or null if there are none.</summary>
    Task<DateOnly?> GetFirstVisitDateAsync(
        CancellationToken ct = default);
}
