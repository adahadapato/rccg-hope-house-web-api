using MediatR;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Entities;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

public class SubmitContactUsCommandHandler : IRequestHandler<SubmitContactUsCommand, Unit>
{
    private readonly IContactUsRepository _repository;
    private readonly IEmailService _emailService;
    private readonly NotificationSettings _settings;

    public SubmitContactUsCommandHandler(
        IContactUsRepository repository,
        IEmailService emailService,
        NotificationSettings settings)
    {
        _repository = repository;
        _emailService = emailService;
        _settings = settings;
    }

    public async Task<Unit> Handle(SubmitContactUsCommand request, CancellationToken ct)
    {
        var contact = Core.Entities.ContactUs.Create(
            request.FirstName, request.LastName, request.Email,
            request.Message, request.Reason, request.PhoneNumber);

        await _repository.AddAsync(contact, ct);
        await _repository.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(_settings.AdminContactEmail))
        {
            _ = NotifyAdminAsync(contact, ct);
        }

        return Unit.Value;
    }

    /// <summary>
    /// Sends admin alert without blocking the user response.
    /// </summary>
    private async Task NotifyAdminAsync(Core.Entities.ContactUs contact, CancellationToken ct)
    {
        var subject = $"New Contact Request: {contact.Reason}";
        var htmlBody = $@"
            <h3>New Contact Form Submission</h3>
            <p><strong>Name:</strong> {contact.FirstName} {contact.LastName}</p>
            <p><strong>Email:</strong> {contact.Email.Value}</p>
            {(contact.PhoneNumber is not null ? $"<p><strong>Phone:</strong> {contact.PhoneNumber.Value}</p>" : "")}
            <p><strong>Reason:</strong> {contact.Reason}</p>
            <p><strong>Message:</strong><br/>{contact.Message}</p>
            <p><em>Submitted on {contact.CreatedAt:MMMM dd, yyyy}</em></p>
        ";

        await _emailService.SendAsync(_settings.AdminContactEmail, subject, htmlBody, ct);
    }
}