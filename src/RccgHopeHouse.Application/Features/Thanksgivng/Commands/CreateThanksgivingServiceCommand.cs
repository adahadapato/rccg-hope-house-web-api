using MediatR;
using RccgHopeHouse.Application.Features.Thanksgiving.Dtos;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Commands;

/// <summary>
/// Command to manually create a thanksgiving service record.
/// Used when auto-sync fails or for historical entries.
/// </summary>
public record CreateThanksgivingServiceCommand(
    string Title,
    string VideoUrl,
    string ThumbnailUrl,
    DateTime ServiceMonth,
    string? Description) : IRequest<ThanksgivingServiceDto>;