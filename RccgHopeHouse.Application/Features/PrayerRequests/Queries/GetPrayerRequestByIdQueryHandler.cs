using MediatR;
using RccgHopeHouse.Application.Features.PrayerRequests.Dtos;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Queries;

/// <summary>
/// Handles retrieval of an individual prayer request.
/// </summary>
public sealed class GetPrayerRequestByIdQueryHandler
    : IRequestHandler<
        GetPrayerRequestByIdQuery,
        PrayerRequestDto>
{
    private readonly IPrayerRequestRepository _repository;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="GetPrayerRequestByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">
    /// Repository used to retrieve prayer requests.
    /// </param>
    public GetPrayerRequestByIdQueryHandler(
        IPrayerRequestRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves the requested prayer request and maps it to
    /// a <see cref="PrayerRequestDto"/>.
    /// </summary>
    /// <param name="request">
    /// The query containing the prayer request identifier.
    /// </param>
    /// <param name="ct">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// The requested prayer request.
    /// </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the requested prayer request does not exist.
    /// </exception>
    public async Task<PrayerRequestDto> Handle(
        GetPrayerRequestByIdQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        var prayer =
            await _repository.GetByIdAsync(
                request.Id,
                ct)
            ?? throw new NotFoundException(
                nameof(PrayerRequest),
                request.Id);

        return PrayerRequestDto.FromEntity(
            prayer);
    }
}