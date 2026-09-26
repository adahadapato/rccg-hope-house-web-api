using RccgHopeHouse.Application.Features.ProphecyCategories.Dtos;
using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

/// <summary>
/// Represents a complete prophecy year including its
/// categories and individual prophecy statements.
/// </summary>
public record ProphecyYearDetailDto(
    Guid Id,
    int Year,
    bool IsPublished,
    IReadOnlyList<ProphecyCategoryDto> Categories)
{
    public static ProphecyYearDetailDto FromEntity(
        ProphecyYear prophecyYear)
    {
        var categories = prophecyYear.Categories
            .OrderBy(category => category.DisplayOrder)
            .ThenBy(category => category.Name)
            .Select(ProphecyCategoryDto.FromEntity)
            .ToList();

        return new ProphecyYearDetailDto(
            Id: prophecyYear.Id,
            Year: prophecyYear.Year,
            IsPublished: prophecyYear.IsPublished,
            Categories: categories);
    }
}
