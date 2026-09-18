using MediatR;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

/// <summary>
/// Handler for updating prayer request status by pastoral staff.
/// </summary>
public class UpdatePrayerRequestStatusCommandHandler : IRequestHandler<UpdatePrayerRequestStatusCommand, Unit>
{
    private readonly IPrayerRequestRepository _repository;

    public UpdatePrayerRequestStatusCommandHandler(IPrayerRequestRepository repository) => _repository = repository;

    /// <summary>
    /// Loads the request, applies status update via domain method, and persists.
    /// </summary>
    public async Task<Unit> Handle(UpdatePrayerRequestStatusCommand request, CancellationToken ct)
    {
        var prayer = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(PrayerRequest), request.Id);

        // Applies the new status and optional pastoral note. No transition
        // restrictions — pastoral staff can freely move a request between any
        // status (e.g., reopening a resolved request if the person follows up
        // again), which fits a small prayer team's workflow better than a rigid
        // state machine would.
        prayer.UpdateStatus(request.NewStatus, request.PastoralNote);

        await _repository.UpdateAsync(prayer, ct);
        await _repository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}