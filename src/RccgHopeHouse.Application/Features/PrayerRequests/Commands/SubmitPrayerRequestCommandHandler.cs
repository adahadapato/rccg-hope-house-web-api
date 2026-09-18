using MediatR;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.PrayerRequests.Commands;

public class SubmitPrayerRequestCommandHandler : IRequestHandler<SubmitPrayerRequestCommand, Unit>
{
    private readonly IPrayerRequestRepository _repository;
    private readonly IEmailService _emailService;
    private readonly NotificationSettings _settings;

    public SubmitPrayerRequestCommandHandler(
        IPrayerRequestRepository repository,
        IEmailService emailService,
        NotificationSettings settings)
    {
        _repository = repository;
        _emailService = emailService;
        _settings = settings;
    }

    public async Task<Unit> Handle(SubmitPrayerRequestCommand request, CancellationToken ct)
    {
        var prayer = PrayerRequest.Create(
            content: request.Content,
            isAnonymous: request.IsAnonymous,
            requesterName: request.RequesterName,
            phoneNumber: request.PhoneNumber,
            requesterEmail: request.RequesterEmail);

        await _repository.AddAsync(prayer, ct);
        await _repository.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(_settings.AdminContactEmail))
        {
            _ = NotifyPrayerTeamAsync(prayer, ct);
        }

        return Unit.Value;
    }

    private async Task NotifyPrayerTeamAsync(PrayerRequest prayer, CancellationToken ct)
    {
        var subject = "New Prayer Request Submitted";
        var htmlBody = $@"
            <h3>New Prayer Request</h3>
            <p><strong>Name:</strong> {prayer.RequesterName}{(prayer.IsAnonymous ? " (submitted anonymously)" : "")}</p>
            {(prayer.RequesterEmail is null ? "" : $"<p><strong>Email:</strong> {prayer.RequesterEmail.Value}</p>")}
            <p><strong>Request:</strong><br/>{prayer.Content}</p>
            <p><em>Submitted on {prayer.CreatedAt:MMMM dd, yyyy}</em></p>
            <p><a href='{_settings.AdminDashboardUrl}/prayer-requests/{prayer.Id}'>View in Admin Panel</a></p>
        ";

        await _emailService.SendAsync(_settings.AdminContactEmail, subject, htmlBody, ct);
    }
}