
using MediatR;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents a request to approve and activate
/// an existing SEO metadata record for public use.
/// </summary>
/// <param name="Id">
/// Unique identifier of the SEO metadata record.
/// </param>
public sealed record ActivateSeoMetadataCommand(
    Guid Id
) : IRequest<bool>;
