
using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Updates devotional content and resolves selected Bible references.
/// Existing optional fields are preserved when omitted.
/// </summary>
public class UpdateDevotionalCommandHandler
    : IRequestHandler<UpdateDevotionalCommand, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;
    private readonly IBibleReferenceService _bibleReferenceService;

    public UpdateDevotionalCommandHandler(
        IDevotionalRepository repository,
        IBibleReferenceService bibleReferenceService)
    {
        _repository = repository;
        _bibleReferenceService = bibleReferenceService;
    }

    public async Task<DevotionalDto> Handle(
        UpdateDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var devotional =
            await _repository.GetByIdForAdminAsync(
                request.Id,
                cancellationToken)
            ?? throw new NotFoundException(
                nameof(Devotional),
                request.Id);

        string? memoryPassageId = null;

        if (request.MemoryVerseReference is not null)
        {
            memoryPassageId =
                string.IsNullOrWhiteSpace(
                    request.MemoryVerseReference)
                    ? ""
                    : DevotionalBibleReferenceParser.BuildPassageId(
                        request.MemoryVerseReference,
                        _bibleReferenceService);
        }

        IReadOnlyList<string>? annualPassageIds = null;
        string? annualDisplayReference =
            request.BibleInOneYearReference;

        if (request.BibleInOneYearReferences is not null)
        {
            annualPassageIds =
                DevotionalBibleReferenceParser.BuildPassageIds(
                    request.BibleInOneYearReferences,
                    _bibleReferenceService);

            annualDisplayReference ??=
                string.Join(
                    ", ",
                    request.BibleInOneYearReferences);
        }
        else if (request.BibleInOneYearReference is not null)
        {
            annualPassageIds =
                string.IsNullOrWhiteSpace(
                    request.BibleInOneYearReference)
                    ? Array.Empty<string>()
                    : DevotionalBibleReferenceParser.BuildPassageIds(
                        new[] { request.BibleInOneYearReference },
                        _bibleReferenceService);
        }

        devotional.Update(
            devotionalDate: request.DevotionalDate,
            theme: request.Theme,
            scriptureReference: request.ScriptureReference,
            passageId: request.PassageId,
            thought: request.Thought,
            commentaryPoints: request.CommentaryPoints,
            prayerPoints: request.PrayerPoints,
            declaration: request.Declaration,
            memoryVerseReference: request.MemoryVerseReference,
            memoryVersePassageId: memoryPassageId,
            bibleInOneYearReference: annualDisplayReference,
            bibleInOneYearPassageIds: annualPassageIds,
            hymnNumber: request.HymnNumber,
            hymnTitle: request.HymnTitle,
            hymnLyrics: request.HymnLyrics,
            additionalReading: request.AdditionalReading,
            keyPoint: request.KeyPoint,
            author: request.Author,
            sourceUrl: request.SourceUrl);

        await _repository.UpdateAsync(
            devotional,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return DevotionalDto.FromEntity(devotional);
    }
}
