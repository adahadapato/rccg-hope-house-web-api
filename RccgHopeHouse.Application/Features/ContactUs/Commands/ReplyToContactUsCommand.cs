using MediatR;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Represents an administrator's reply to a Contact Us submission.
/// </summary>
/// <param name="ContactId">
/// The unique identifier of the Contact Us submission being answered.
/// </param>
/// <param name="Subject">
/// The subject of the reply email.
/// </param>
/// <param name="Body">
/// The message to send to the original requester.
/// </param>
/// <remarks>
/// Replies are sent through the RCCG Hope House Info mailbox.
/// When delivery succeeds, the associated Contact Us submission
/// is marked as responded.
/// </remarks>
public record ReplyToContactUsCommand(
    Guid ContactId,
    string Subject,
    string Body) : IRequest<Unit>;