using MediatR;


using RccgHopeHouse.Application.Features.Seo.Dtos;


/// <summary>
/// Retrieves an SEO metadata record by its identifier.
/// Intended for administration operations.
/// </summary>
/// <param name="Id">
/// Unique identifier of the SEO metadata record.
/// </param>
public sealed record GetSeoMetadataByIdQuery(
    Guid Id
) : IRequest<SeoMetadataDto?>;
