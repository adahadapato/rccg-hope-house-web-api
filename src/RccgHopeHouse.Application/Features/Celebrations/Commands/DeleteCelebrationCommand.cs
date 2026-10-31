
using MediatR;

namespace RccgHopeHouse.Application.Features.Celebrations.Commands;

/// <summary>
/// Represents a request to soft-delete an existing celebration.
/// </summary>
/// <param name="Id">
/// The unique identifier of the celebration to delete.
/// </param>
/// <remarks>
/// This command performs a logical deletion rather than
/// permanently removing the celebration from the database.
///
/// Published celebrations must be withdrawn through the
/// appropriate withdrawal workflow before any further
/// administrative retention or deletion decisions.
///
/// The command is intended for authorised administrators.
/// </remarks>
public sealed record DeleteCelebrationCommand(
    Guid Id
) : IRequest<bool>;
