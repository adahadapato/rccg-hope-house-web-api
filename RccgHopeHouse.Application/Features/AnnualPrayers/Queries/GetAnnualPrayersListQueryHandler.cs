using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

/// <summary>
/// Handles <see cref="GetAnnualPrayersListQuery"/>.
/// </summary>
public sealed class GetAnnualPrayersListQueryHandler
    : IRequestHandler<
        GetAnnualPrayersListQuery,
        IReadOnlyList<AnnualPrayerDto>>
{
    private readonly IAnnualPrayerRepository
        _annualPrayerRepository;

    public GetAnnualPrayersListQueryHandler(
        IAnnualPrayerRepository annualPrayerRepository)
    {
        _annualPrayerRepository =
            annualPrayerRepository;
    }

    public async Task<IReadOnlyList<AnnualPrayerDto>> Handle(
        GetAnnualPrayersListQuery request,
        CancellationToken cancellationToken)
    {
        var annualPrayers =
            await _annualPrayerRepository
                .GetAllAsync(
                    cancellationToken);

        return annualPrayers
            .Where(prayer =>
                !prayer.IsDeleted)
            .Select(prayer =>
                new AnnualPrayerDto(
                    prayer.Id,
                    prayer.Year,
                    prayer.Theme,
                    prayer.Service,
                    prayer.Author,
                    prayer.BibleReference,
                    prayer.BibleText,
                    prayer.Declaration,
                    prayer.ClosingVerse,
                    prayer.ImageUrl,
                    prayer.IsActive,
                    prayer.PrayerPoints
                        .Where(point =>
                            !point.IsDeleted)
                        .OrderBy(point =>
                            point.DisplayOrder)
                        .Select(point =>
                            new AnnualPrayerPointDto(
                                point.Id,
                                point.Text,
                                point.DisplayOrder))
                        .ToList()))
            .ToList();
    }
}