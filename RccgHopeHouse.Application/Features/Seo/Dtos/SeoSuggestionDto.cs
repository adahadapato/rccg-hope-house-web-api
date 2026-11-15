
namespace RccgHopeHouse.Application.Features.Seo.Dtos;

public sealed record SeoSuggestionDto(
    string SeoTitle,
    string MetaDescription,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> SuggestedHeadings,
    IReadOnlyList<string> Improvements,
    int SeoScore,
    string ScoreExplanation
);
