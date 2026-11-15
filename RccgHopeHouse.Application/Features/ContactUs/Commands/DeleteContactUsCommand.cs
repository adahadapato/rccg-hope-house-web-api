using MediatR;

namespace RccgHopeHouse.Application.Features.ContactUs.Commands;

/// <summary>
/// Command to permanently delete a contact request.
/// Use with caution: removes data from the database.
/// </summary>
public record DeleteContactUsCommand(Guid Id) : IRequest<Unit>;