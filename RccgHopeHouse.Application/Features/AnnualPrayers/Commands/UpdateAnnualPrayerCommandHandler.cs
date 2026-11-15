using MediatR;
using RccgHopeHouse.Application.Features.AnnualPrayers.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.AnnualPrayers.Commands;

public class UpdateAnnualPrayerCommandHandler
    : IRequestHandler<
        UpdateAnnualPrayerCommand,
        AnnualPrayerDto>
{
    private readonly IAnnualPrayerRepository _repository;

    public UpdateAnnualPrayerCommandHandler(
        IAnnualPrayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<AnnualPrayerDto> Handle(
        UpdateAnnualPrayerCommand request,
        CancellationToken cancellationToken)
    {
        var annualPrayer =
            await _repository.GetByIdAsync(
                request.Id,
                cancellationToken)
            ?? throw new NotFoundException(
                nameof(AnnualPrayer),
                request.Id);

        if (annualPrayer.Year != request.Year)
        {
            var existingForYear =
                await _repository.GetByYearAsync(
                    request.Year,
                    cancellationToken);

            if (existingForYear is not null &&
                existingForYear.Id !=
                annualPrayer.Id)
            {
                throw new InvalidOperationException(
                    $"An annual prayer already exists for {request.Year}.");
            }
        }

        annualPrayer.Update(
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

        var existingPrayerPoints =
            annualPrayer.PrayerPoints
                .ToList();

        if (existingPrayerPoints.Count > 0)
        {
            await _repository
                .RemovePrayerPointsAsync(
                    existingPrayerPoints,
                    cancellationToken);

            annualPrayer.PrayerPoints.Clear();
        }

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

        if (annualPrayer.IsActive)
        {
            await _repository
                .DeactivateOtherActiveAsync(
                    annualPrayer.Id,
                    cancellationToken);
        }

        await _repository.UpdateAsync(
            annualPrayer,
            cancellationToken);

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