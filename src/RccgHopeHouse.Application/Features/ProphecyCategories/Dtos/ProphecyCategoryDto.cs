using RccgHopeHouse.Application.Features.Prophecies.Dtos;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;

/// <summary>
/// Represents a prophecy category and its prophecy statements.
/// </summary>
public record ProphecyCategoryDto(
    Guid Id,
    Guid ProphecyYearId,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsActive,
    IReadOnlyList<ProphecyDto> Prophecies)
{
    public static ProphecyCategoryDto FromEntity(
        ProphecyCategory category)
    {
        var prophecies = category.Prophecies
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.CreatedAt)
            .Select(ProphecyDto.FromEntity)
            .ToList();

        return new ProphecyCategoryDto(
            Id: category.Id,
            ProphecyYearId: category.ProphecyYearId,
            Name: category.Name,
            Description: category.Description,
            DisplayOrder: category.DisplayOrder,
            IsActive: category.IsActive,
            Prophecies: prophecies);
    }
}
