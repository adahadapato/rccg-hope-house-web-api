using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.Offerings.Dtos;

/// <summary>
/// Represents an offering submitted through the Give Online feature.
/// </summary>
public record OfferingDto(
    Guid Id,
    Guid GivingTypeId,
    string GivingTypeName,
    decimal Amount,
    PaymentStatus PaymentStatus,
    string? PaymentReference,
    string? FullName,
    string? Email,
    bool IsAnonymous,
    string? MessageReference,
    DateTime CreatedAt)
{
    /// <summary>
    /// Creates an offering DTO from an <see cref="Offering"/> entity.
    /// </summary>
    public static OfferingDto FromEntity(Offering offering) => new(
        Id: offering.Id,
        GivingTypeId: offering.GivingTypeId,
        GivingTypeName: offering.GivingType?.Name ?? string.Empty,
        Amount: offering.Amount,
        PaymentStatus: offering.PaymentStatus,
        PaymentReference: offering.PaymentReference,
        FullName: offering.FullName,
        Email: offering.Email?.Value,
        IsAnonymous: offering.IsAnonymous,
        MessageReference: offering.MessageReference,
        CreatedAt: offering.CreatedAt);
}