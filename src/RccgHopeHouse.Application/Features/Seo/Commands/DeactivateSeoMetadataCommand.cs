
using MediatR;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents an administrator's request to deactivate
/// an existing SEO metadata record.
/// </summary>
/// <remarks>
/// Deactivation prevents the metadata from being returned
/// by public SEO queries without deleting the record.
/// </remarks>
/// <param name="Id">
/// Unique identifier of the SEO metadata record.
/// </param>
public sealed record DeactivateSeoMetadataCommand(
    Guid Id
) : IRequest<bool>;
