using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Core.Interfaces.AI;

/// <summary>
/// Defines SEO recommendation functionality for website content.
/// </summary>
public interface ISeoService
{
    /// <summary>
    /// Generates SEO recommendations for the supplied content.
    /// </summary>
    Task<SeoSuggestionResult> GenerateAsync(
        string contentType,
        string topic,
        string? content,
        CancellationToken cancellationToken = default);
}
