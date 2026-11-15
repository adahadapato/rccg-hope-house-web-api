using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Commands;

public record CreateAnnualPrayerPointRequest(
    string Text,
    int DisplayOrder);

public record CreateAnnualPrayerCommand(
    int Year,
    string Theme,
    string Service,
    string Author,
    string BibleReference,
    string BibleText,
    string Declaration,
    string ClosingVerse,
    string? ImageUrl,
    bool IsActive,
    IReadOnlyList<CreateAnnualPrayerPointRequest> PrayerPoints)
    : IRequest<AnnualPrayerDto>;