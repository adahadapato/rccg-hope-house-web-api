
using MediatR;
using RccgHopeHouse.Application.Features.Devotionals.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Creates a devotional and automatically resolves additional
/// scripture references to validated Bible passage IDs.
/// </summary>
public class CreateDevotionalCommandHandler
    : IRequestHandler<CreateDevotionalCommand, DevotionalDto>
{
    private readonly IDevotionalRepository _repository;
    private readonly IBibleReferenceService _bibleReferenceService;

    public CreateDevotionalCommandHandler(
        IDevotionalRepository repository,
        IBibleReferenceService bibleReferenceService)
    {
        _repository = repository;
        _bibleReferenceService = bibleReferenceService;
    }

    public async Task<DevotionalDto> Handle(
        CreateDevotionalCommand request,
        CancellationToken cancellationToken)
    {
        var memoryReference =
            request.MemoryVerseReference?.Trim() ?? "";

        var memoryPassageId =
            string.IsNullOrWhiteSpace(memoryReference)
                ? ""
                : DevotionalBibleReferenceParser.BuildPassageId(
                    memoryReference,
                    _bibleReferenceService);

        var annualReferences =
            request.BibleInOneYearReferences ??
            (string.IsNullOrWhiteSpace(
                request.BibleInOneYearReference)
                ? Array.Empty<string>()
                : new[] { request.BibleInOneYearReference! });

        var annualPassageIds =
            DevotionalBibleReferenceParser.BuildPassageIds(
                annualReferences,
                _bibleReferenceService);

        var annualDisplayReference =
            request.BibleInOneYearReference ??
            string.Join(", ", annualReferences);

        var devotional = Devotional.Create(
            devotionalDate: request.DevotionalDate,
            theme: request.Theme,
            scriptureReference: request.ScriptureReference,
            passageId: request.PassageId,
            thought: request.Thought,
            commentaryPoints: request.CommentaryPoints,
            prayerPoints: request.PrayerPoints,
            declaration: request.Declaration,
            memoryVerseReference: memoryReference,
            memoryVersePassageId: memoryPassageId,
            bibleInOneYearReference: annualDisplayReference,
            bibleInOneYearPassageIds: annualPassageIds,
            hymnNumber: request.HymnNumber ?? "",
            hymnTitle: request.HymnTitle ?? "",
            hymnLyrics: request.HymnLyrics ?? "",
            additionalReading: request.AdditionalReading ?? "",
            keyPoint: request.KeyPoint ?? "",
            author: request.Author ?? "",
            sourceUrl: request.SourceUrl ?? "");

        await _repository.AddAsync(
            devotional,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return DevotionalDto.FromEntity(devotional);
    }
}
