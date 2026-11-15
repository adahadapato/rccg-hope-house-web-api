using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Commands;

public class CreateAnnualPrayerCommandHandler
    : IRequestHandler<
        CreateAnnualPrayerCommand,
        AnnualPrayerDto>
{
    private readonly IAnnualPrayerRepository _repository;

    public CreateAnnualPrayerCommandHandler(
        IAnnualPrayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnualPrayerDto> Handle(
        CreateAnnualPrayerCommand request,
        CancellationToken cancellationToken)
    {
        var existing =
            await _repository.GetByYearAsync(
                request.Year,
                cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"An annual prayer already exists for {request.Year}.");
        }

        var annualPrayer =
            AnnualPrayer.Create(
                year: request.Year,
                theme: request.Theme,
                service: request.Service,
                author: request.Author,
                bibleReference:
                    request.BibleReference,
                bibleText:
                    request.BibleText,
                declaration:
                    request.Declaration,
                closingVerse:
                    request.ClosingVerse,
                imageUrl:
                    request.ImageUrl,
                isActive:
                    request.IsActive);

        foreach (var point in
                 request.PrayerPoints
                     .OrderBy(
                         point =>
                             point.DisplayOrder))
        {
            var prayerPoint =
                AnnualPrayerPoint.Create(
                    annualPrayerId:
                        annualPrayer.Id,
                    text:
                        point.Text,
                    displayOrder:
                        point.DisplayOrder);

            annualPrayer.PrayerPoints.Add(
                prayerPoint);
        }

        await _repository.AddAsync(
            annualPrayer,
            cancellationToken);

        if (annualPrayer.IsActive)
        {
            await _repository
                .DeactivateOtherActiveAsync(
                    annualPrayer.Id,
                    cancellationToken);
        }

        await _repository.SaveChangesAsync(
            cancellationToken);

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