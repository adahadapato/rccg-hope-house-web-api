namespace RccgHopeHouse.Application.Features.SiteVisits.Dtos;

/// <summary>
/// Figures for the "Website Overview" panel on the admin dashboard.
/// A change value is null when the earlier period has nothing to
/// compare with.
/// </summary>
public record SiteOverviewDto(
    int Days,
    int Visitors,
    int PageViews,
    int AverageSeconds,
    int MobilePercent,
    double? VisitorsChangePercent,
    double? PageViewsChangePercent,
    double? AverageSecondsChangePercent,
    double? MobileChangePoints,
    int ActiveNow,
    DateOnly? CountingSince,
    IReadOnlyList<SiteOverviewDayDto> Daily);

public record SiteOverviewDayDto(
    DateOnly Date,
    int Visitors,
    int PageViews);
