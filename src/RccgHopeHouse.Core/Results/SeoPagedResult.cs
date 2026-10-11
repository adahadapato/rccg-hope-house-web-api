namespace RccgHopeHouse.Core.Results;

/// <summary>
/// Represents a paginated collection of SEO metadata records.
/// </summary>
/// <typeparam name="T">
/// Type of record returned in the collection.
/// </typeparam>
public sealed record SeoPagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);
