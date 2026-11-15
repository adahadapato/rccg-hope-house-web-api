
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Handles administrative requests to retrieve celebrations by workflow stage.
/// </summary>
/// <remarks>
/// Uses the existing ICelebrationRepository methods without introducing
/// additional repository operations or modifying database configuration.
///
/// Date-sensitive queries use the United Kingdom's local calendar date.
/// </remarks>
public sealed class GetCelebrationsQueryHandler : IRequestHandler<GetCelebrationsQuery, IReadOnlyList<CelebrationDto>>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes a new instance of the celebration listing handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve celebration records.</param>
    public GetCelebrationsQueryHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Retrieves celebrations matching the requested workflow stage and date range.
    /// </summary>
    /// <param name="request">The celebration listing request.</param>
    /// <param name="ct">Cancellation token for repository operations.</param>
    /// <returns>A read-only collection of celebration DTOs.</returns>
    /// <exception cref="ArgumentException">Thrown when the requested status or date range is invalid.</exception>
    public async Task<IReadOnlyList<CelebrationDto>> Handle(GetCelebrationsQuery request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var status = string.IsNullOrWhiteSpace(request.Status) ? "Upcoming" : request.Status.Trim();

        var today = GetCurrentUkDate();
        var fromDate = request.FromDate ?? today;
        var throughDate = request.ThroughDate ?? fromDate.AddDays(30);

        if (fromDate == default || throughDate == default || throughDate < fromDate)
        {
            throw new ArgumentException("The celebration date range is invalid.");
        }

        IReadOnlyList<Celebration> celebrations;

        switch (status)
        {
            case "Upcoming":
                celebrations = await _celebrationRepository.GetUpcomingAsync(fromDate, throughDate, ct);
                break;

            case "Pending":
                celebrations = await _celebrationRepository.GetUnsentInvitationsAsync(fromDate, throughDate, ct);
                break;

            case "AwaitingMemberResponse":
                celebrations = await _celebrationRepository.GetAwaitingMemberResponseAsync(ct);
                break;

            case "ApprovedByMember":
                celebrations = await _celebrationRepository.GetAwaitingAdminApprovalAsync(ct);
                break;

            case "ReadyToPublish":
                celebrations = await _celebrationRepository.GetReadyToPublishAsync(request.ThroughDate ?? today, ct);
                break;

            case "Published":
                celebrations = await _celebrationRepository.GetPublishedAsync(fromDate, throughDate, ct);
                break;

            default:
                throw new ArgumentException($"Unsupported celebration status: {status}", nameof(request.Status));
        }

        // The current listing repository methods do not consistently load
        // Member and GalleryCategory navigation properties.
        // Reload each celebration using GetByIdAsync so the administrative
        // DTO contains the related member and category information.
        var results = new List<CelebrationDto>(celebrations.Count);

        foreach (var celebration in celebrations)
        {
            ct.ThrowIfCancellationRequested();

            var detailedCelebration = await _celebrationRepository.GetByIdAsync(celebration.Id, ct);

            if (detailedCelebration is not null)
            {
                results.Add(CelebrationDto.FromEntity(detailedCelebration));
            }
        }

        return results;
    }

    /// <summary>
    /// Retrieves the current UK calendar date, accounting for GMT and BST.
    /// </summary>
    /// <returns>The current date in the United Kingdom.</returns>
    private static DateOnly GetCurrentUkDate()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
        var ukNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        return DateOnly.FromDateTime(ukNow);
    }
}
