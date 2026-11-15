using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Queries;

/// <summary>
/// Handles <see cref="GetAnnualPrayerByIdQuery"/>.
/// </summary>
public sealed class GetAnnualPrayerByIdQueryHandler
    : IRequestHandler<
        GetAnnualPrayerByIdQuery,
        AnnualPrayerDto?>
{
    private readonly IAnnualPrayerRepository
        _annualPrayerRepository;

    public GetAnnualPrayerByIdQueryHandler(
        IAnnualPrayerRepository annualPrayerRepository)
    {
        _annualPrayerRepository =
            annualPrayerRepository;
    }

    public async Task<AnnualPrayerDto?> Handle(
        GetAnnualPrayerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var annualPrayer =
            await _annualPrayerRepository
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

        if (annualPrayer is null ||
            annualPrayer.IsDeleted)
        {
            return null;
        }

        var prayerPoints =
            annualPrayer.PrayerPoints
                .Where(point =>
                    !point.IsDeleted)
                .OrderBy(point =>
                    point.DisplayOrder)
                .Select(point =>
                    new AnnualPrayerPointDto(
                        point.Id,
                        point.Text,
                        point.DisplayOrder))
                .ToList();

        return new AnnualPrayerDto(
            annualPrayer.Id,
            annualPrayer.Year,
            annualPrayer.Theme,
            annualPrayer.Service,
            annualPrayer.Author,
            annualPrayer.BibleReference,
            annualPrayer.BibleText,
            annualPrayer.Declaration,
            annualPrayer.ClosingVerse,
            annualPrayer.ImageUrl,
            annualPrayer.IsActive,
            prayerPoints);
    }
}