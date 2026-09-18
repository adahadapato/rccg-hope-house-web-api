using MediatR;
using RccgHopeHouse.Application.Features.ChurchInfo.Dtos;

namespace RccgHopeHouse.Application.Features.ChurchInfo.Commands;

public record UpdateChurchAboutSectionCommand(
    string ParishName,
    int EstablishedYear,
    string Tagline,
    string AboutLead,
    string AboutText,
    string MultiCulturalStat) : IRequest<ChurchInfoDto>;