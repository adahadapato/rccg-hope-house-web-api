
using MediatR;
using RccgHopeHouse.Application.Features.Seo.Dtos;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents a request to generate SEO recommendations
/// for website content.
/// </summary>
public sealed record GenerateSeoCommand(
    string ContentType,
    string Topic,
    string? Content
) : IRequest<SeoSuggestionDto>;
