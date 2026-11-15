using MediatR;

namespace RccgHopeHouse.Application.Features.ChurchServices.Commands;

/// <summary>
/// Command to permanently delete a service schedule.
/// Use with caution: removes all associated data from the database.
/// </summary>
public record DeleteChurchServiceCommand(Guid Id) : IRequest<Unit>;