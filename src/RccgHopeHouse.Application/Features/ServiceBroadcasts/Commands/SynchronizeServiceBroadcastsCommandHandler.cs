/*This class helps synchronize service broadcasts from configured YouTube channels
 using AI to clean broadcast descriptions*/

using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Enums;
using RccgHopeHouse.Core.Interfaces;
using RccgHopeHouse.Core.Interfaces.AI;
using RccgHopeHouse.Core.Results;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Commands;

/// <summary>
/// Handles synchronization of service broadcasts from configured
/// YouTube channels.
/// </summary>
public sealed class SynchronizeServiceBroadcastsCommandHandler
    : IRequestHandler<
        SynchronizeServiceBroadcastsCommand,
        ServiceBroadcastSynchronizationResult>
{
    private const int RecentVideoLimit = 50;
    private const int UpcomingVideoLimit = 25;

    private static readonly string[] ExcerptKeywords =
    [
        "sermon",
        "message",
        "excerpt",
        "clip",
        "highlights",
        "highlight"
    ];

    private readonly IChurchServiceRepository _churchServiceRepository;
    private readonly IServiceBroadcastRepository _serviceBroadcastRepository;
    private readonly IYouTubeService _youTubeService;
    private readonly IServiceBroadcastDescriptionCleaner
        _descriptionCleaner;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="SynchronizeServiceBroadcastsCommandHandler"/> class.
    /// </summary>
    /// <param name="churchServiceRepository">
    /// Repository used to retrieve broadcast-enabled church services.
    /// </param>
    /// <param name="serviceBroadcastRepository">
    /// Repository used to create and update service broadcasts.
    /// </param>
    /// <param name="youTubeService">
    /// Service used to retrieve configured YouTube channels
    /// and public YouTube video information.
    /// </param>
    /// <param name="descriptionCleaner">
    /// Service used to clean YouTube broadcast descriptions and
    /// extract service themes before they are stored.
    /// </param>
    public SynchronizeServiceBroadcastsCommandHandler(
        IChurchServiceRepository churchServiceRepository,
        IServiceBroadcastRepository serviceBroadcastRepository,
        IYouTubeService youTubeService,
        IServiceBroadcastDescriptionCleaner descriptionCleaner)
    {
        _churchServiceRepository = churchServiceRepository;
        _serviceBroadcastRepository = serviceBroadcastRepository;
        _youTubeService = youTubeService;
        _descriptionCleaner = descriptionCleaner;
    }

    /// <summary>
    /// Synchronizes broadcasts for active church services that have
    /// broadcast synchronization enabled.
    /// </summary>
    /// <param name="request">
    /// The synchronization command.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A summary containing the number of videos examined and broadcasts
    /// created, updated, or skipped.
    /// </returns>
    public async Task<ServiceBroadcastSynchronizationResult> Handle(
        SynchronizeServiceBroadcastsCommand request,
        CancellationToken cancellationToken)
    {
        var churchServices =
            await _churchServiceRepository
                .GetBroadcastEnabledAsync(
                    cancellationToken);

        if (churchServices.Count == 0)
        {
            return new ServiceBroadcastSynchronizationResult(
                0,
                0,
                0,
                0);
        }

        var channels =
            _youTubeService.GetChannels();

        if (channels.Count == 0)
        {
            return new ServiceBroadcastSynchronizationResult(
                0,
                0,
                0,
                0);
        }

        var discoveredVideos =
            new Dictionary<string, YouTubeVideoInfo>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var channel in channels)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var recentVideos =
                await _youTubeService.GetChannelVideosAsync(
                    channel.ChannelId,
                    RecentVideoLimit,
                    cancellationToken);

            AddVideos(
                discoveredVideos,
                recentVideos);

            var upcomingVideos =
                await _youTubeService
                    .GetUpcomingLiveStreamsAsync(
                        channel.ChannelId,
                        UpcomingVideoLimit,
                        cancellationToken);

            AddVideos(
                discoveredVideos,
                upcomingVideos);
        }

        var created = 0;
        var updated = 0;
        var skipped = 0;

        /*
         * Process full-service candidates before sermons,
         * excerpts, clips and highlights.
         */
        var orderedVideos =
            discoveredVideos.Values
                .OrderBy(IsExcerptOrClip)
                .ThenByDescending(GetEventDate)
                .ToList();

        foreach (var video in orderedVideos)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var matchingService =
                FindMatchingService(
                    video,
                    churchServices);

            if (matchingService is null)
            {
                skipped++;
                continue;
            }

            /*
             * Sermons, excerpts, clips and highlights should not
             * become the primary broadcast for a church service.
             */
            if (IsExcerptOrClip(video))
            {
                skipped++;
                continue;
            }

            var existingByVideoId =
                await _serviceBroadcastRepository
                    .GetByVideoIdAsync(
                        video.VideoId,
                        cancellationToken);

            if (existingByVideoId is not null)
            {
                var existingDescription =
                    existingByVideoId.Description;

                var existingTheme =
                    existingByVideoId.Theme;

                /*
                 * Do not send an already processed broadcast to
                 * OpenAI on every hourly synchronization.
                 *
                 * Process it when either its description or theme
                 * is still missing.
                 */
                if ((string.IsNullOrWhiteSpace(
                         existingDescription) ||
                     string.IsNullOrWhiteSpace(
                         existingTheme)) &&
                    (!string.IsNullOrWhiteSpace(
                         video.Title) ||
                     !string.IsNullOrWhiteSpace(
                         video.Description)))
                {
                    var cleaningResult =
                        await _descriptionCleaner.CleanAsync(
                            video.Title,
                            video.Description,
                            cancellationToken);

                    if (string.IsNullOrWhiteSpace(
                            existingDescription))
                    {
                        existingDescription =
                            cleaningResult.Description;
                    }

                    if (!string.IsNullOrWhiteSpace(
                            cleaningResult.Theme))
                    {
                        existingTheme =
                            cleaningResult.Theme;
                    }
                }

                if (UpdateExistingBroadcastIfRequired(
                    existingByVideoId,
                    video,
                    existingDescription,
                    existingTheme))
                {
                    await _serviceBroadcastRepository
                        .UpdateAsync(
                            existingByVideoId,
                            cancellationToken);

                    await _serviceBroadcastRepository
                        .SaveChangesAsync(
                            cancellationToken);

                    updated++;
                }
                else
                {
                    skipped++;
                }

                continue;
            }

            var eventDate =
                GetEventDate(video);

            var serviceMonth =
                new DateTime(
                    eventDate.Year,
                    eventDate.Month,
                    1);

            var existingForMonth =
                await _serviceBroadcastRepository
                    .GetByServiceAndMonthAsync(
                        matchingService.Id,
                        serviceMonth,
                        cancellationToken);

            if (existingForMonth is not null)
            {
                /*
                 * This is a different YouTube video for the same
                 * church service and month.
                 *
                 * Process the new video so that both its cleaned
                 * description and its actual service theme can be
                 * stored against the monthly broadcast.
                 */
                var cleaningResult =
                    await _descriptionCleaner.CleanAsync(
                        video.Title,
                        video.Description,
                        cancellationToken);

                var updatedTheme =
                    cleaningResult.Theme ??
                    existingForMonth.Theme ??
                    matchingService.CurrentTheme;

                existingForMonth.Update(
                    video.Title,
                    video.VideoUrl,
                    cleaningResult.Description,
                    updatedTheme,
                    IsLive(video));

                await _serviceBroadcastRepository
                    .UpdateAsync(
                        existingForMonth,
                        cancellationToken);

                await _serviceBroadcastRepository
                    .SaveChangesAsync(
                        cancellationToken);

                updated++;
                continue;
            }

            /*
             * This is a completely new monthly broadcast.
             *
             * Process the YouTube title and description so that the
             * cleaned description and service-specific theme are
             * stored with the broadcast.
             */
            var newBroadcastMetadata =
                await _descriptionCleaner.CleanAsync(
                    video.Title,
                    video.Description,
                    cancellationToken);

            var broadcast =
                ServiceBroadcast.Create(
                    matchingService.Id,
                    matchingService.Category,
                    video.Title,
                    video.VideoUrl,
                    serviceMonth,
                    newBroadcastMetadata.Description,
                    newBroadcastMetadata.Theme ??
                    matchingService.CurrentTheme,
                    IsLive(video));

            await _serviceBroadcastRepository
                .AddAsync(
                    broadcast,
                    cancellationToken);

            await _serviceBroadcastRepository
                .SaveChangesAsync(
                    cancellationToken);

            created++;
        }

        return new ServiceBroadcastSynchronizationResult(
            discoveredVideos.Count,
            created,
            updated,
            skipped);
    }

    /// <summary>
    /// Adds discovered videos to the collection while preventing
    /// duplicate YouTube video identifiers.
    /// </summary>
    /// <param name="destination">
    /// Collection receiving discovered videos.
    /// </param>
    /// <param name="videos">
    /// Videos returned by YouTube.
    /// </param>
    private static void AddVideos(
        IDictionary<string, YouTubeVideoInfo> destination,
        IEnumerable<YouTubeVideoInfo> videos)
    {
        foreach (var video in videos)
        {
            if (string.IsNullOrWhiteSpace(
                    video.VideoId))
            {
                continue;
            }

            destination[video.VideoId] =
                video;
        }
    }

    /// <summary>
    /// Attempts to match a YouTube video to one of the church
    /// services currently enabled for broadcast synchronization.
    /// </summary>
    /// <param name="video">
    /// The discovered YouTube video.
    /// </param>
    /// <param name="churchServices">
    /// Broadcast-enabled church services.
    /// </param>
    /// <returns>
    /// The matching church service, or <see langword="null"/>
    /// when no reliable match can be determined.
    /// </returns>
    private static ChurchService? FindMatchingService(
        YouTubeVideoInfo video,
        IReadOnlyList<ChurchService> churchServices)
    {
        var normalizedTitle =
            Normalize(video.Title);

        /*
         * Prefer the configured ChurchService name.
         *
         * This allows database configuration to drive matching
         * whenever the service name appears in the YouTube title.
         */
        var serviceByName =
            churchServices
                .Where(service =>
                    !string.IsNullOrWhiteSpace(
                        service.Name))
                .OrderByDescending(service =>
                    Normalize(service.Name).Length)
                .FirstOrDefault(service =>
                    normalizedTitle.Contains(
                        Normalize(service.Name),
                        StringComparison.Ordinal));

        if (serviceByName is not null)
        {
            return serviceByName;
        }

        /*
         * Fall back to known textual representations of the
         * ServiceCategory when the configured service name differs
         * from the wording used by YouTube.
         */
        return churchServices.FirstOrDefault(
            service =>
                MatchesCategory(
                    normalizedTitle,
                    service.Category));
    }

    /// <summary>
    /// Determines whether a normalized YouTube title contains
    /// wording associated with a service category.
    /// </summary>
    /// <param name="normalizedTitle">
    /// Normalized YouTube video title.
    /// </param>
    /// <param name="category">
    /// Church service category to test.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the title matches the category;
    /// otherwise <see langword="false"/>.
    /// </returns>
    private static bool MatchesCategory(
        string normalizedTitle,
        ServiceCategory category)
    {
        IReadOnlyList<string> terms =
            category switch
            {
                ServiceCategory.WednesdayPrayer =>
                    ["wednesday prayer"],

                ServiceCategory.SundaySchool =>
                    ["sunday school"],

                ServiceCategory.WorshipService =>
                    ["worship service"],

                ServiceCategory.ThanksgivingService =>
                [
                    "thanksgiving service",
                    "thanksgiving"
                ],

                ServiceCategory.LastFridayVigil =>
                [
                    "last friday vigil",
                    "friday vigil"
                ],

                ServiceCategory.Evangelism =>
                    ["evangelism"],

                ServiceCategory.HouseFellowship =>
                    ["house fellowship"],

                ServiceCategory.SpecialEvent =>
                    ["special event"],

                ServiceCategory.HolyCommunion =>
                [
                    "holy communion service",
                    "holy communion"
                ],

                ServiceCategory.HolyGhostService =>
                    ["holy ghost service"],

                ServiceCategory.DivineEncounter =>
                    ["divine encounter"],

                _ =>
                    []
            };

        return terms.Any(
            term =>
                normalizedTitle.Contains(
                    term,
                    StringComparison.Ordinal));
    }

    /// <summary>
    /// Determines whether a discovered video appears to be an
    /// excerpt, sermon, clip or highlight rather than the complete
    /// service.
    /// </summary>
    /// <param name="video">
    /// The YouTube video to inspect.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the video appears to be partial
    /// content; otherwise <see langword="false"/>.
    /// </returns>
    private static bool IsExcerptOrClip(
        YouTubeVideoInfo video)
    {
        var normalizedTitle =
            Normalize(video.Title);

        return ExcerptKeywords.Any(
            keyword =>
                normalizedTitle.Contains(
                    keyword,
                    StringComparison.Ordinal));
    }

    /// <summary>
    /// Determines the most reliable event date available for a
    /// YouTube broadcast.
    /// </summary>
    /// <param name="video">
    /// The YouTube video.
    /// </param>
    /// <returns>
    /// Actual start time when available, otherwise scheduled start
    /// time, and finally the YouTube publication time.
    /// </returns>
    private static DateTime GetEventDate(
        YouTubeVideoInfo video)
    {
        return video.ActualStartTime
            ?? video.ScheduledStartTime
            ?? video.PublishedAt;
    }

    /// <summary>
    /// Determines whether the YouTube broadcast is currently live.
    /// </summary>
    /// <param name="video">
    /// The YouTube video.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when YouTube reports the broadcast
    /// as live; otherwise <see langword="false"/>.
    /// </returns>
    private static bool IsLive(
        YouTubeVideoInfo video)
    {
        return video.BroadcastStatus ==
               YouTubeBroadcastStatus.Live;
    }

    /// <summary>
    /// Updates an existing service broadcast when its YouTube
    /// metadata, cleaned description, extracted theme or live
    /// status has changed.
    /// </summary>
    /// <param name="broadcast">
    /// Existing service broadcast.
    /// </param>
    /// <param name="video">
    /// Latest YouTube video information.
    /// </param>
    /// <param name="description">
    /// Description that should remain associated with the broadcast.
    /// </param>
    /// <param name="theme">
    /// Theme that should remain associated with the broadcast.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when an update was required;
    /// otherwise <see langword="false"/>.
    /// </returns>
    private static bool UpdateExistingBroadcastIfRequired(
        ServiceBroadcast broadcast,
        YouTubeVideoInfo video,
        string? description,
        string? theme)
    {
        var isLive =
            IsLive(video);

        var currentDescription =
            NormalizeOptionalText(
                broadcast.Description);

        var updatedDescription =
            NormalizeOptionalText(
                description);

        var currentTheme =
            NormalizeOptionalText(
                broadcast.Theme);

        var updatedTheme =
            NormalizeOptionalText(
                theme);

        /*
         * Deliberately do not compare the stored description with
         * video.Description.
         *
         * The stored description may have been cleaned by OpenAI,
         * while video.Description contains the original YouTube
         * text. Comparing them would cause unnecessary updates
         * every hour.
         */
        var needsUpdate =
            !string.Equals(
                broadcast.Title,
                video.Title.Trim(),
                StringComparison.Ordinal) ||
            !string.Equals(
                currentDescription,
                updatedDescription,
                StringComparison.Ordinal) ||
            !string.Equals(
                currentTheme,
                updatedTheme,
                StringComparison.Ordinal) ||
            broadcast.IsLive != isLive;

        if (!needsUpdate)
        {
            return false;
        }

        broadcast.Update(
            video.Title,
            video.VideoUrl,
            updatedDescription,
            updatedTheme,
            isLive);

        return true;
    }

    /// <summary>
    /// Normalizes optional text by trimming it and converting
    /// blank values to <see langword="null"/>.
    /// </summary>
    /// <param name="value">
    /// Text to normalize.
    /// </param>
    /// <returns>
    /// Normalized text.
    /// </returns>
    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    /// <summary>
    /// Normalizes text so service names can be compared reliably
    /// with YouTube video titles.
    /// </summary>
    /// <param name="value">
    /// Text to normalize.
    /// </param>
    /// <returns>
    /// Lower-case text with punctuation converted to spaces and
    /// repeated whitespace removed.
    /// </returns>
    private static string Normalize(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var characters =
            value
                .Trim()
                .ToLowerInvariant()
                .Select(character =>
                    char.IsLetterOrDigit(character)
                        ? character
                        : ' ')
                .ToArray();

        return string.Join(
            ' ',
            new string(characters)
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries));
    }
}