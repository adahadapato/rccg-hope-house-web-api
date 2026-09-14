using MediatR;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Command to mark a contact request as read in the admin inbox.
/// </summary>
public record MarkContactUsAsReadCommand(Guid Id) : IRequest<Unit>;