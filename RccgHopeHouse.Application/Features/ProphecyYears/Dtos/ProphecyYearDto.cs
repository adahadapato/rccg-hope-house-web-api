using RccgHopeHouse.Core.Entities;

namespace RccgHopeHouse.Application.Features.ProphecyYears.Dtos;

/// <summary>
/// Represents a prophecy year.
/// </summary>
public record ProphecyYearDto(
    Guid Id,
    int Year,
    bool IsPublished)
{
    public static ProphecyYearDto FromEntity(
        ProphecyYear prophecyYear) => new(
            Id: prophecyYear.Id,
            Year: prophecyYear.Year,
            IsPublished: prophecyYear.IsPublished);
}