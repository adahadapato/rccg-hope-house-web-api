using MediatR;
using RccgHopeHouse.Application.Features.Offerings.Dtos;

namespace RccgHopeHouse.Application.Features.Offerings.Commands;

/// <summary>
/// Creates a new offering submitted through the Give Online feature.
/// </summary>
public record CreateOfferingCommand(
    Guid GivingTypeId,
    decimal Amount,
    bool IsAnonymous,
    string? FullName,
    string? Email,
    string? MessageReference) : IRequest<OfferingDto>;