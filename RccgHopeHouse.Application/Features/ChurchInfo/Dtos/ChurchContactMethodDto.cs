using RccgHopeHouse.Core.Enums;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

public record ChurchContactMethodDto(
    Guid Id,
    ContactMethodType Type,
    string Value,
    string? Label,
    int DisplayOrder)
{
    public static ChurchContactMethodDto FromEntity(Core.Entities.ChurchContactMethod method) => new(
        Id: method.Id,
        Type: method.Type,
        Value: method.Value,
        Label: method.Label,
        DisplayOrder: method.DisplayOrder);
}
