
using MediatR;

namespace RccgHopeHouse.Application.Features.Seo.Commands;

/// <summary>
/// Represents an administrator's request to delete
/// an existing SEO metadata record.
/// </summary>
/// <param name="Id">
/// Unique identifier of the SEO metadata record.
/// </param>
public sealed record DeleteSeoMetadataCommand(
    Guid Id
) : IRequest<bool>;
