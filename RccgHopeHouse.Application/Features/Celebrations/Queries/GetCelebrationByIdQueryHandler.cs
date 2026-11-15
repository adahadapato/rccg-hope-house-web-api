
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Queries;

/// <summary>
/// Handles requests to retrieve an individual celebration.
/// </summary>
public sealed class GetCelebrationByIdQueryHandler
    : IRequestHandler<GetCelebrationByIdQuery, CelebrationDto?>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes the celebration retrieval handler.
    /// </summary>
    /// <param name="celebrationRepository">
    /// Repository responsible for celebration queries.
    /// </param>
    public GetCelebrationByIdQueryHandler(
        ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository
            ?? throw new ArgumentNullException(
                nameof(celebrationRepository));
    }

    /// <summary>
    /// Retrieves a celebration and maps it to its DTO.
    /// </summary>
    /// <param name="request">
    /// The query containing the celebration identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// The celebration DTO, or null if no matching
    /// celebration exists.
    /// </returns>
    public async Task<CelebrationDto?> Handle(
        GetCelebrationByIdQuery request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid celebration ID is required.",
                nameof(request.Id));
        }

        var celebration =
            await _celebrationRepository.GetByIdAsync(
                request.Id,
                ct);

        return celebration is null
            ? null
            : CelebrationDto.FromEntity(celebration);
    }
}
