using MediatR;
using RccgHopeHouse.Application.Features.ServiceBroadcasts.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ServiceBroadcasts.Queries;

/// <summary>
/// Handles requests for the latest public service broadcasts.
/// </summary>
/// <remarks>
/// Broadcast eligibility is controlled by the corresponding
/// <c>ChurchService.IsBroadcastEnabled</c> setting.
///
/// The handler deliberately contains no hard-coded service
/// categories. This allows administrators to decide which
/// individual church services support broadcasts without
/// requiring application code changes.
/// </remarks>
public class GetLatestServiceBroadcastsQueryHandler
    : IRequestHandler<
        GetLatestServiceBroadcastsQuery,
        IReadOnlyList<ServiceBroadcastDto>>
{
    private readonly IServiceBroadcastRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetLatestServiceBroadcastsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve service broadcasts.
    /// </param>
    public GetLatestServiceBroadcastsQueryHandler(
        IServiceBroadcastRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves the latest broadcast for every active church
    /// service that has broadcasting enabled.
    /// </summary>
    /// <param name="request">
    /// Query requesting the latest public service broadcasts.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// DTOs representing the latest broadcasts for eligible
    /// church services.
    /// </returns>
    public async Task<IReadOnlyList<ServiceBroadcastDto>> Handle(
        GetLatestServiceBroadcastsQuery request,    CancellationToken ct)
    {
        var broadcasts =  await _repository.GetLatestForBroadcastEnabledServicesAsync(ct);

        return broadcasts.Select(ServiceBroadcastDto.FromEntity)
            .ToList();
    }
}