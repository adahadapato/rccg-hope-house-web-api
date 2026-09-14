using MediatR;
using RccgHopeHouse.Application.Features.Thanksgiving.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Thanksgiving.Queries;

/// <summary>
/// Handler for fetching paginated thanksgiving services.
/// </summary>
public class GetThanksgivingFeedQueryHandler : IRequestHandler<GetThanksgivingFeedQuery, IReadOnlyList<ThanksgivingServiceDto>>
{
    private readonly IThanksgivingRepository _repository;

    public GetThanksgivingFeedQueryHandler(IThanksgivingRepository repository) => _repository = repository;

    /// <summary>
    /// Fetches services from repository and maps to DTOs.
    /// Orders by ServiceMonth descending (newest first).
    /// </summary>
    public async Task<IReadOnlyList<ThanksgivingServiceDto>> Handle(
        GetThanksgivingFeedQuery request,
        CancellationToken cancellationToken)
    {
        var services = await _repository.GetAsync(
            fromMonth: DateTime.MinValue,
            toMonth: DateTime.MaxValue,
            ct: cancellationToken);

        return services
            .OrderByDescending(s => s.ServiceMonth)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(s => new ThanksgivingServiceDto(
                s.Id, s.Title, s.VideoUrl, s.ThumbnailUrl, s.ServiceMonth, s.IsAutoSynced))
            .ToList();
    }
}