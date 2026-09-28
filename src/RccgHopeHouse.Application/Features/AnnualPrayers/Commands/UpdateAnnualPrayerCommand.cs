using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Commands;

public record UpdateAnnualPrayerPointRequest(
    string Text,
    int DisplayOrder);

public record UpdateAnnualPrayerCommand(
    Guid Id,
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
    IReadOnlyList<UpdateAnnualPrayerPointRequest> PrayerPoints)
    : IRequest<AnnualPrayerDto>;