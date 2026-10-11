using MediatR;


using RccgHopeHouse.Application.Features.Seo.Dtos;


/// <summary>
/// Retrieves approved SEO metadata for an existing content record.
/// </summary>
/// <param name="ContentType">
/// Content category, such as Sermon or Event.
/// </param>
/// <param name="ContentId">
/// Identifier of the associated content.
/// </param>
public sealed record GetActiveSeoMetadataByContentQuery(
    string ContentType,
    Guid ContentId
) : IRequest<SeoMetadataDto?>;