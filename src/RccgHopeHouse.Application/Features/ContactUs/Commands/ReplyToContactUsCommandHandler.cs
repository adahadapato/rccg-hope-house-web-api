using MediatR;
using RccgHopeHouse.Core.Constants;
using RccgHopeHouse.Core.Exceptions;
using RccgHopeHouse.Core.Interfaces;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Handler for sending email replies to contact form submissions.
/// Integrates with IEmailService for SMTP/transactional delivery.
/// </summary>
public class ReplyToContactUsCommandHandler : IRequestHandler<ReplyToContactUsCommand, Unit>
{
    private readonly IContactUsRepository _repository;
    private readonly IEmailService _emailService;
    private readonly NotificationSettings _settings;

    public ReplyToContactUsCommandHandler(
        IContactUsRepository repository,
        IEmailService emailService,
        NotificationSettings settings)
    {
        _repository = repository;
        _emailService = emailService;
        _settings = settings;
    }

    public async Task<Unit> Handle(ReplyToContactUsCommand request, CancellationToken ct)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, ct)
            ?? throw new NotFoundException(nameof(Core.Entities.ContactUs), request.ContactId);

        var adminName = _settings.AdminContactName ?? "RCCG Hope House Team";
        var htmlBody = $@"
            <p>Dear {contact.FirstName},</p>
            <p>{request.Body.Replace("\n", "<br/>")}</p>
            <p>Best regards,<br/>{adminName}</p>
        ";

        //await _emailService.SendAsync(contact.Email, request.Subject, htmlBody, ct);
        await _emailService.SendAsync(contact.Email.Value, request.Subject, htmlBody, ct);
        return Unit.Value;
    }
}