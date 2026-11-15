namespace RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

public record ChurchInfoDto(
    Guid Id,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string? PostCode,
    string Country,
    string ParishName,
    int EstablishedYear,
    int YearsOfMinistry,
    string Tagline,
    string AboutLead,
    string AboutText,
    string MultiCulturalStat,
    int ActiveMemberCount,
    IReadOnlyList<ChurchContactMethodDto> ContactMethods)
{
    /// <summary>
    /// Maps the entity plus a live member count (fetched separately via
    /// IMemberRepository.GetActiveCountAsync) into the response DTO —
    /// ActiveMemberCount is never stored on ChurchInfo itself.
    /// </summary>
    public static ChurchInfoDto FromEntity(Core.Entities.ChurchInfo info, int activeMemberCount) => new(
        Id: info.Id,
        AddressLine1: info.AddressLine1,
        AddressLine2: info.AddressLine2,
        City: info.City,
        PostCode: info.PostCode,
        Country: info.Country,
        ParishName: info.ParishName,
        EstablishedYear: info.EstablishedYear,
        YearsOfMinistry: info.YearsOfMinistry,
        Tagline: info.Tagline,
        AboutLead: info.AboutLead,
        AboutText: info.AboutText,
        MultiCulturalStat: info.MultiCulturalStat,
        ActiveMemberCount: activeMemberCount,
        ContactMethods: info.ContactMethods
            .OrderBy(m => m.DisplayOrder)
            .Select(ChurchContactMethodDto.FromEntity)
            .ToList());
}