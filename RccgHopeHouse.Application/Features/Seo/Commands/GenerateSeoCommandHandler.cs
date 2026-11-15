
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;
using RccgHopeHouse.Core.Interfaces.AI;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Handles SEO generation requests using the SEO service.
/// </summary>
public sealed class GenerateSeoCommandHandler : IRequestHandler<GenerateSeoCommand, SeoSuggestionDto>
{
    private readonly ISeoService _seoService;

    public GenerateSeoCommandHandler(ISeoService seoService)
    {
        _seoService = seoService;
    }

    public async Task<SeoSuggestionDto> Handle(GenerateSeoCommand request, CancellationToken cancellationToken)
    {
        var result = await _seoService.GenerateAsync(
            request.ContentType,
            request.Topic,
            request.Content,
            cancellationToken);

        return new SeoSuggestionDto(
            SeoTitle: result.SeoTitle,
            MetaDescription: result.MetaDescription,
            Keywords: result.Keywords,
            SuggestedHeadings: result.SuggestedHeadings,
            Improvements: result.Improvements,
            SeoScore: result.SeoScore,
            ScoreExplanation: result.ScoreExplanation);
    }
}
