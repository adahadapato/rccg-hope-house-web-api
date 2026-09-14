using MediatR;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands
{
    /// <summary>
    /// Command to send an email reply to a contact requester.
    /// Does not modify the contact entity; only triggers email delivery.
    /// </summary>
    public record ReplyToContactUsCommand(Guid ContactId, string Subject, string Body) : IRequest<Unit>;

}
