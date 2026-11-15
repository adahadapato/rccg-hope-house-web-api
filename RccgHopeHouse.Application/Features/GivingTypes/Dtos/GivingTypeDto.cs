using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.GivingTypes.Dtos;

/// <summary>
/// Represents a giving type available within the application,
/// such as Tithe, Offering, Seed Offering or Building Fund.
/// </summary>
public record GivingTypeDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    int DisplayOrder)
{
    /// <summary>
    /// Creates a DTO from a GivingType domain entity.
    /// </summary>
    public static GivingTypeDto FromEntity(GivingType givingType) => new(
        Id: givingType.Id,
        Name: givingType.Name,
        Description: givingType.Description,
        IsActive: givingType.IsActive,
        DisplayOrder: givingType.DisplayOrder);
}