using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.Prophecies.Dtos;

/// <summary>
/// Represents an individual prophecy statement.
/// </summary>
public record ProphecyDto(
    Guid Id,
    Guid CategoryId,
    string Text,
    int DisplayOrder,
    bool IsActive)
{
    public static ProphecyDto FromEntity(
        Prophecy prophecy) => new(
            Id: prophecy.Id,
            CategoryId: prophecy.CategoryId,
            Text: prophecy.Text,
            DisplayOrder: prophecy.DisplayOrder,
            IsActive: prophecy.IsActive);
}