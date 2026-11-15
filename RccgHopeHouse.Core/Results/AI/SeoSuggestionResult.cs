namespace RccgHopeHouse.Core.Results.AI;

/// <summary>
/// SEO recommendations for a website content item.
/// </summary>
public sealed record SeoSuggestionResult(
    string SeoTitle,
    string MetaDescription,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> SuggestedHeadings,
    IReadOnlyList<string> Improvements,
    int SeoScore,
    string ScoreExplanation
);
