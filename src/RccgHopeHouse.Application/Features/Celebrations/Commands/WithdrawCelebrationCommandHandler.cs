
using MediatR;
using RccgHopeHouse.Application.Features.Celebrations.Dtos;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Handles withdrawal of publication permission for member celebrations.
/// </summary>
/// <remarks>
/// Withdrawal is permitted for celebrations that have already received
/// member approval, administrator approval or publication.
///
/// The celebration record and historical timestamps are retained for auditing.
/// The public website must exclude withdrawn celebrations immediately.
/// </remarks>
public sealed class WithdrawCelebrationCommandHandler : IRequestHandler<WithdrawCelebrationCommand, CelebrationDto>
{
    private readonly ICelebrationRepository _celebrationRepository;

    /// <summary>
    /// Initializes a new instance of the withdrawal handler.
    /// </summary>
    /// <param name="celebrationRepository">Repository used to retrieve and update celebrations.</param>
    public WithdrawCelebrationCommandHandler(ICelebrationRepository celebrationRepository)
    {
        _celebrationRepository = celebrationRepository ?? throw new ArgumentNullException(nameof(celebrationRepository));
    }

    /// <summary>
    /// Withdraws publication permission and persists the updated celebration.
    /// </summary>
    /// <param name="request">The withdrawal command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated celebration DTO.</returns>
    /// <exception cref="ArgumentException">Thrown when the celebration identifier is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the celebration cannot be found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when withdrawal is not permitted.</exception>
    public async Task<CelebrationDto> Handle(WithdrawCelebrationCommand request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CelebrationId == Guid.Empty)
            throw new ArgumentException("A valid celebration ID is required.", nameof(request.CelebrationId));

        var celebration = await _celebrationRepository.GetByIdAsync(request.CelebrationId, ct);

        if (celebration is null)
            throw new KeyNotFoundException("The selected celebration does not exist.");

        if (celebration.Status is not ("ApprovedByMember" or "ReadyToPublish" or "Published"))
            throw new InvalidOperationException("Only an approved or published celebration can be withdrawn.");

        celebration.Withdraw(DateTime.UtcNow);

        await _celebrationRepository.UpdateAsync(celebration, ct);
        await _celebrationRepository.SaveChangesAsync(ct);

        return CelebrationDto.FromEntity(celebration);
    }
}
