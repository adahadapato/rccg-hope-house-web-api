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

        // Domain method ensures status transitions follow church workflow rules
        prayer.UpdateStatus(request.NewStatus, request.PastoralNote);

        await _repository.UpdateAsync(prayer, ct);
        await _repository.SaveChangesAsync(ct);

        return Unit.Value;
    }
}