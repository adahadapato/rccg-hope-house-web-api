
using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.AI;
using RccgHopeHouse.Core.Results.AI;

namespace RccgHopeHouse.Application.Features.Devotionals.Commands;

/// <summary>
/// Synchronises Open Heavens devotional content with the existing
/// devotional repository, including additional Bible references,
/// hymns, key points and source information.
/// </summary>
public sealed class SynchronizeOpenHeavensDevotionalsCommandHandler
    : IRequestHandler<
        SynchronizeOpenHeavensDevotionalsCommand,
        OpenHeavensDevotionalSynchronizationResult>
{
    private readonly IDevotionalRepository _repository;
    private readonly IOpenHeavensDevotionalService _openHeavensService;
    private readonly IBibleReferenceService _bibleReferenceService;

    private static readonly TimeZoneInfo UkTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/London");

    /// <summary>
    /// Initialises the devotional synchronisation handler.
    /// </summary>
    public SynchronizeOpenHeavensDevotionalsCommandHandler(
        IDevotionalRepository repository,
        IOpenHeavensDevotionalService openHeavensService,
        IBibleReferenceService bibleReferenceService)
    {
        _repository = repository;
        _openHeavensService = openHeavensService;
        _bibleReferenceService = bibleReferenceService;
    }

    /// <summary>
    /// Retrieves, validates, saves and, where appropriate,
    /// publishes the requested devotional.
    /// </summary>
    public async Task<OpenHeavensDevotionalSynchronizationResult> Handle(
        SynchronizeOpenHeavensDevotionalsCommand request,
        CancellationToken cancellationToken)
    {
        var today = GetUkToday();

        // Never automatically publish a future devotional.
        if (request.DevotionalDate > today)
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                "Future devotional synchronisation is not permitted.");
        }

        // Preserve existing devotional records.
        var existing = await _repository.GetByDateForAdminAsync(
            request.DevotionalDate,
            cancellationToken);

        if (existing is not null)
        {
            if (existing.IsPublished)
            {
                return Result(
                    request.DevotionalDate,
                    "AlreadyPublished",
                    "The devotional is already published.");
            }

            // Publish today's existing draft without overwriting
            // manually edited content.
            if (request.DevotionalDate == GetUkToday())
            {
                existing.Publish();

                await _repository.UpdateAsync(
                    existing,
                    cancellationToken);

                await _repository.SaveChangesAsync(
                    cancellationToken);

                return Result(
                    request.DevotionalDate,
                    "Published",
                    "An existing devotional draft was published.");
            }

            return Result(
                request.DevotionalDate,
                "AlreadyExists",
                "A draft already exists for this historical date.");
        }

        // Retrieve the devotional using the existing AI service.
        var content = await _openHeavensService.GetDevotionalAsync(
            request.DevotionalDate,
            cancellationToken);

        if (content is null)
        {
            return Result(
                request.DevotionalDate,
                "NotFound",
                "Open Heavens content was not available.");
        }

        if (content.DevotionalDate != request.DevotionalDate)
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                "The retrieved devotional date does not match the requested date.");
        }

        // Validate and generate the main Bible reading passage ID.
        string passageId;

        try
        {
            passageId = DevotionalBibleReferenceParser.BuildPassageId(
                content.ScriptureReference,
                _bibleReferenceService);
        }
        catch (ArgumentException)
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                $"Unsupported scripture reference: {content.ScriptureReference}");
        }

        // Validate and generate the memory verse passage ID.
        var memoryVerseReference =
            content.MemoryVerseReference?.Trim() ?? "";

        var memoryVersePassageId = "";

        if (!string.IsNullOrWhiteSpace(memoryVerseReference))
        {
            try
            {
                memoryVersePassageId =
                    DevotionalBibleReferenceParser.BuildPassageId(
                        memoryVerseReference,
                        _bibleReferenceService);
            }
            catch (ArgumentException)
            {
                return Result(
                    request.DevotionalDate,
                    "InvalidContent",
                    $"Unsupported memory verse reference: {memoryVerseReference}");
            }
        }

        // Generate all Bible in One Year passage IDs.
        var annualReferences = content.BibleInOneYearReferences;

        if (annualReferences is null || annualReferences.Count == 0)
        {
            annualReferences =
                string.IsNullOrWhiteSpace(content.BibleInOneYearReference)
                    ? Array.Empty<string>()
                    : new[] { content.BibleInOneYearReference };
        }

        IReadOnlyList<string> annualPassageIds;

        try
        {
            annualPassageIds =
                DevotionalBibleReferenceParser.BuildPassageIds(
                    annualReferences,
                    _bibleReferenceService);
        }
        catch (ArgumentException)
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                "The Bible in One Year reference contains an unsupported book, chapter or verse.");
        }

        var annualDisplayReference =
            !string.IsNullOrWhiteSpace(content.BibleInOneYearReference)
                ? content.BibleInOneYearReference.Trim()
                : string.Join(", ", annualReferences);

        // Validate required devotional content.
        if (string.IsNullOrWhiteSpace(content.Theme) ||
            string.IsNullOrWhiteSpace(content.Thought) ||
            content.CommentaryPoints is null ||
            content.CommentaryPoints.Count == 0 ||
            content.PrayerPoints is null ||
            content.PrayerPoints.Count == 0)
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                "The devotional is missing one or more required fields.");
        }

        // The AI generates an original theme-based declaration when
        // the source does not contain one. Reject an empty result
        // rather than saving a placeholder as devotional content.
        if (string.IsNullOrWhiteSpace(content.Declaration))
        {
            return Result(
                request.DevotionalDate,
                "InvalidContent",
                "The devotional declaration is missing.");
        }

        var declaration = content.Declaration.Trim();

        // Recheck before insertion to reduce duplicate attempts.
        if (await _repository.ExistsForDateAsync(
                devotionalDate: request.DevotionalDate,
                ct: cancellationToken))
        {
            return Result(
                request.DevotionalDate,
                "AlreadyExists",
                "A devotional was created by another operation.");
        }

        var devotional = Devotional.Create(
            devotionalDate: content.DevotionalDate,
            theme: content.Theme,
            scriptureReference: content.ScriptureReference,
            passageId: passageId,
            thought: content.Thought,
            commentaryPoints: content.CommentaryPoints,
            prayerPoints: content.PrayerPoints,
            declaration: declaration,
            memoryVerseReference: memoryVerseReference,
            memoryVersePassageId: memoryVersePassageId,
            bibleInOneYearReference: annualDisplayReference,
            bibleInOneYearPassageIds: annualPassageIds,
            hymnNumber: content.HymnNumber,
            hymnTitle: content.HymnTitle,
            hymnLyrics: content.HymnLyrics,
            additionalReading: content.AdditionalReading ?? "",
            keyPoint: content.KeyPoint ?? "",
            author: content.Author ?? "",
            sourceUrl: content.SourceUrl ?? "");

        // Recalculate UK today after the external request.
        var publishToday =
            request.DevotionalDate == GetUkToday();

        if (publishToday)
        {
            devotional.Publish();
        }

        await _repository.AddAsync(
            devotional,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result(
            request.DevotionalDate,
            publishToday ? "Published" : "Created",
            publishToday
                ? "Today's devotional was created and published."
                : "The devotional was saved as an unpublished draft.");
    }

    /// <summary>
    /// Returns today's date in the United Kingdom.
    /// </summary>
    private static DateOnly GetUkToday()
    {
        var now = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            UkTimeZone);

        return DateOnly.FromDateTime(now.DateTime);
    }

    /// <summary>
    /// Creates a consistent synchronisation result.
    /// </summary>
    private static OpenHeavensDevotionalSynchronizationResult Result(
        DateOnly date,
        string status,
        string message)
    {
        return new OpenHeavensDevotionalSynchronizationResult(
            date,
            status,
            message);
    }
}
